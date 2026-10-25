"""Integration checks; run from any directory after dotnet build."""
import hashlib
from html.parser import HTMLParser
import json
from pathlib import Path
import shutil
import subprocess
import tempfile
from urllib.parse import unquote, urlsplit

ROOT = Path(__file__).resolve().parents[1]
DLL = ROOT / 'Pond/bin/Debug/net8.0/Pond.dll'


def run(cwd, *args, ok=True):
    result = subprocess.run(['dotnet', str(DLL), *map(str, args)], cwd=cwd,
                            capture_output=True, text=True)
    assert (result.returncode == 0) == ok, result.stdout + result.stderr
    return result


class Links(HTMLParser):
    def __init__(self):
        super().__init__()
        self.urls = []
        self.ids = set()

    def handle_starttag(self, tag, attrs):
        attrs = dict(attrs)
        self.urls.extend(attrs[k] for k in ('href', 'src') if k in attrs)
        if 'id' in attrs:
            self.ids.add(attrs['id'])


def snapshot(site):
    return {p.relative_to(site).as_posix(): hashlib.sha256(p.read_bytes()).hexdigest()
            for p in site.rglob('*') if p.is_file()}


with tempfile.TemporaryDirectory(prefix='pond test ') as temporary:
    work = Path(temporary)
    project = work / 'project'
    shutil.copytree(ROOT / 'examples/basic', project, ignore=shutil.ignore_patterns('site'))
    run(work, 'build', project / 'config.json')
    site = project / 'site'
    assert set(snapshot(site)) == {'index.html', 'guide/start.html', 'guide/deep/details.html', 'style/site.css'}
    for page in site.rglob('*.html'):
        parser = Links()
        parser.feed(page.read_text())
        for url in parser.urls:
            parts = urlsplit(url)
            if parts.scheme or parts.netloc:
                continue
            target = (page.parent / unquote(parts.path)).resolve() if parts.path else page
            assert target.is_file(), (page, url)
            if parts.fragment:
                destination = Links()
                destination.feed(target.read_text())
                assert unquote(parts.fragment) in destination.ids, (page, url)
    assert '<ul>' in (site / 'index.html').read_text()
    assert '&lt;hello&gt;' in (site / 'index.html').read_text()
    assert '&lt;p&gt;Code stays text.&lt;/p&gt;' in (site / 'guide/start.html').read_text()
    assert '"Template"' not in (site / 'guide/deep/details.html').read_text()
    first = snapshot(site)
    shutil.rmtree(site)
    run(project, 'build', '/c', 'config.json')
    assert first == snapshot(site), 'Clean rebuild changed bytes'
    run(work, 'build', project / 'config.json')
    assert first == snapshot(site), 'Repeated build changed bytes'

    template = project / 'templates/page.html'
    # Exact EOF, whitespace preservation, nested scopes and names containing "in".
    template.write_text('A {{ TITLE }} B{% for index in ELEMENTS %}{% if index != null %}x{% end %}{% for index in CHAPTERS %}y{% endfor %}{{ index.ToHtml() }}{% endfor %}Z')
    run(work, 'build', project / 'config.json')
    output = (site / 'index.html').read_text()
    assert output.startswith('A Pond example Bx') and output.endswith('Z'), output
    template.write_text('{{ TITLE }}')
    run(work, 'build', project / 'config.json')
    assert (site / 'index.html').read_text() == 'Pond example'
    for invalid, diagnostic in [
        ('{{ TITLE', 'Unterminated'), ('{% if true %}', 'Missing endif'),
        ('{% for x in ELEMENTS %}{% endif %}', 'Unexpected endif'),
        ('{% end %}', 'Unexpected end'), ('{% wat %}', 'Unknown action'),
        ('{% for x ELEMENTS %}', 'Expected for'), ('{{ }}', 'Empty'),
        ('{{ UNKNOWN }}', 'Failed to evaluate'),
        ('{% if UNKNOWN %}x{% end %}', 'Failed to evaluate'),
        ('{% for x in UNKNOWN %}x{% end %}', 'Failed to evaluate'),
        ('{% if 1 %}x{% end %}', 'must be boolean'),
        ('{% for x in 1 %}x{% end %}', 'not enumerable'),
        ('{% for x in ELEMENTS %}{% end %}{{ x }}', 'Failed to evaluate'),
    ]:
        template.write_text(invalid)
        result = run(work, 'build', project / 'config.json', ok=False)
        assert diagnostic in result.stderr and 'page.html' in result.stderr, result.stderr
    template.write_text('{{ TITLE }}')
    config = json.loads((project / 'config.json').read_text())
    for key in config:
        broken = dict(config)
        del broken[key]
        (project / 'invalid.json').write_text(json.dumps(broken))
        assert key in run(work, 'build', project / 'invalid.json', ok=False).stderr
    broken = dict(config, Typo=True)
    (project / 'invalid.json').write_text(json.dumps(broken))
    assert 'Typo' in run(work, 'build', project / 'invalid.json', ok=False).stderr
    (project / 'articles/index.md').write_text('<!-- {"Template":"missing"} -->\n\n# Missing')
    assert 'Template not found' in run(work, 'build', project / 'config.json', ok=False).stderr
    (project / 'articles/index.md').write_text('<!-- {"Template":  } -->\n\n# Invalid')
    assert 'Invalid article metadata' in run(work, 'build', project / 'config.json', ok=False).stderr
    fresh = work / 'fresh'
    fresh.mkdir()
    run(fresh, 'init')
    assert json.loads((fresh / 'config.json').read_text()) == dict(config, DefaultArticleInfo={'Template': 'index'})
    before = (fresh / 'config.json').read_bytes()
    run(fresh, 'init', ok=False)
    assert before == (fresh / 'config.json').read_bytes()
    run(work, 'unknown', ok=False)
    run(work, 'build', 'missing.json', ok=False)
print('Verified fixture links, deterministic bytes, configuration, scopes and failure diagnostics.')
