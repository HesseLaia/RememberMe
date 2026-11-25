const fs = require('node:fs');
const path = require('node:path');
const {execFileSync} = require('node:child_process');
const root = path.resolve(__dirname, '..');
const git = (...args) => execFileSync('git', args, {cwd: root, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe']});
const files = [...new Set((git('ls-files', '-z', '--', '*.md') +
  git('ls-files', '--others', '--exclude-standard', '-z', '--', '*.md')).split('\0'))]
  .filter(name => name && fs.existsSync(path.join(root, name)));
const errors = [];
let pairs = 0;
for (const name of files) {
  const chinese = name.endsWith('.zh-CN.md');
  const counterpart = chinese ? name.replace(/\.zh-CN\.md$/, '.md') : name.replace(/\.md$/, '.zh-CN.md');
  if (!fs.existsSync(path.join(root, counterpart))) {errors.push(name + ' missing ' + counterpart); continue;}
  if (!chinese) pairs++;
  const text = fs.readFileSync(path.join(root, name), 'utf8');
  const links = [...text.matchAll(/\]\(([^)]+)\)/g)].map(match => match[1].replace(/^<|>$/g, '').split('#')[0]);
  if (!links.some(target => !/^[a-z]+:/i.test(target) &&
      path.resolve(root, path.dirname(name), decodeURIComponent(target)) === path.resolve(root, counterpart)))
    errors.push(name + ' missing reciprocal language link');
  const heading = text.match(/^#\s+(.+)$/m)?.[1] || '';
  if (!heading) errors.push(name + ' missing document title');
  if (!chinese && /[\u3400-\u9fff]/u.test(heading)) errors.push(name + ' has a Chinese default title');
  for (const target of links) {
    if (!target || /^[a-z]+:/i.test(target)) continue;
    if (!fs.existsSync(path.resolve(root, path.dirname(name), decodeURIComponent(target))))
      errors.push(name + ' -> ' + target);
  }
}
console.log(JSON.stringify({documents: files.length, pairs, status: errors.length ? 'failed' : 'passed'}));
for (const error of errors) console.error(error);
process.exitCode = errors.length ? 1 : 0;
