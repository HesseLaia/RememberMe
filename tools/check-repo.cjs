const fs = require('node:fs');
const path = require('node:path');
const {execFileSync} = require('node:child_process');
const root = path.resolve(__dirname, '..');
const git = (...args) => execFileSync('git', args, {cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']}).trim();
const lines = text => text ? text.split(/\r?\n/) : [];
const release = JSON.parse(fs.readFileSync(path.join(root, 'release.json'), 'utf8'));
let comparisonBase = 'origin/main';
try {comparisonBase = git('rev-parse', '--abbrev-ref', '--symbolic-full-name', '@{upstream}');}
catch {try {git('rev-parse', '--verify', comparisonBase);} catch {comparisonBase = 'HEAD';}}
const changed = [...new Set([...lines(git('diff', '--name-only', comparisonBase)),
  ...lines(git('ls-files', '--others', '--exclude-standard'))])];
const gameRoots = ['RememberMeUnity/Assets', 'RememberMeUnity/Packages', 'RememberMeUnity/ProjectSettings'];
const unpublishedGameFiles = [...new Set([...lines(git('diff', '--name-only', release.sourceCommit, '--', ...gameRoots)),
  ...changed.filter(file => gameRoots.some(prefix => file.startsWith(prefix + '/')))])];
const documents = [...new Set(['README.md', 'README.zh-CN.md', 'CONTRIBUTING.md', 'docs/README.md',
  'docs/HANDOFF.zh-CN.md', 'docs/UPLOAD.zh-CN.md', 'RememberMeUnity/README.md', 'archive/README.md',
  ...changed.filter(file => file.endsWith('.md') && fs.existsSync(path.join(root, file)))])];
const errors = [];
let languages;
try {
  languages = JSON.parse(execFileSync(process.execPath, [path.join(root, 'tools/check-languages.cjs')],
    {cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']}));
} catch (error) {
  errors.push('Bilingual documentation check failed: ' + (error.stderr?.toString() || error.message));
}
for (const name of documents) {
  const file = path.join(root, name);
  if (!fs.existsSync(file)) {errors.push('Missing document ' + name); continue;}
  const text = fs.readFileSync(file, 'utf8');
  for (const match of text.matchAll(/\]\(([^)]+)\)/g)) {
    const target = match[1].replace(/^<|>$/g, '').split('#')[0];
    if (!target || /^[a-z]+:/i.test(target)) continue;
    if (!fs.existsSync(path.resolve(path.dirname(file), decodeURIComponent(target))))
      errors.push(name + ' -> ' + target);
  }
}
for (const name of changed.filter(file => file.endsWith('.cjs') && fs.existsSync(path.join(root, file)))) {
  try {execFileSync(process.execPath, ['--check', path.join(root, name)], {stdio: 'pipe'});}
  catch (error) {errors.push(name + ': ' + error.stderr.toString());}
}
const webRoot = path.resolve(root, release.webRoot);
if (!webRoot.startsWith(root + path.sep)) errors.push('Release path leaves the repository');
else {
  const index = path.join(webRoot, 'index.html');
  if (!fs.existsSync(index) || !fs.existsSync(path.join(webRoot, '.nojekyll'))) errors.push('Missing release entry or .nojekyll');
  else {
    const html = fs.readFileSync(index, 'utf8');
    const assets = [...html.matchAll(/(?:dataUrl|frameworkUrl|codeUrl|loader\.src)\s*[:=]\s*['"]([^'"]+)['"]/g)];
    if (assets.length !== 4) errors.push('Expected four Unity build assets');
    for (const [, name] of assets) {
      const file = path.join(webRoot, name);
      if (!fs.existsSync(file)) errors.push('Missing web asset ' + name);
      else if (fs.statSync(file).size >= 100 * 1024 * 1024) errors.push('Web asset exceeds GitHub file limit ' + name);
    }
  }
}
if (!fs.existsSync(path.join(root, release.verification)) ||
    !fs.readFileSync(path.join(root, release.verification), 'utf8').includes('REMEMBER_WEB_VERIFIED'))
  errors.push('Missing browser verification record');
const routingChanged = changed.some(file => /^(404\.html|archive\/|tools\/check-archive-links\.cjs)/.test(file));
const webEntryChanged = changed.some(file => ['index.html', 'unity/index.html', 'release.json'].includes(file) ||
  file.startsWith(release.webRoot + '/') || file.startsWith('releases/web/'));
const report = {branch: git('branch', '--show-current'), head: git('rev-parse', '--short', 'HEAD'),
  comparisonBase,
  languages,
  publishedRelease: release.version, changedFiles: changed, unpublishedGameFiles,
  requiredChecks: [routingChanged && 'Archive browser redirects', webEntryChanged && 'Public web entry check',
    unpublishedGameFiles.length && 'Fresh game build and full browser QA'].filter(Boolean),
  staticChecks: errors.length ? 'failed' : 'passed', errors};
const reportPath = path.join(root, 'RememberMeUnity/Build/Review/upload-report.json');
fs.mkdirSync(path.dirname(reportPath), {recursive: true});
fs.writeFileSync(reportPath, JSON.stringify(report, null, 2) + '\n');
console.log(JSON.stringify({branch: report.branch, head: report.head, release: release.version,
  changedFiles: changed.length, unpublishedGameFiles: unpublishedGameFiles.length,
  languages,
  requiredChecks: report.requiredChecks, staticChecks: report.staticChecks}));
for (const error of errors) console.error(error);
process.exitCode = errors.length ? 1 : 0;
