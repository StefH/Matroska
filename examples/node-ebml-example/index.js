const fs = require('fs');
const { Decoder } = require('./node_modules/ebml/lib/ebml');

const decoder = new Decoder();

decoder.on('data', (chunk) => console.log(chunk));

fs.readFile('issue16.webm', (err, data) => {
  if (err) {
    throw err;
  }
  decoder.write(data);
});