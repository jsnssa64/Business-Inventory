const fs = require('fs')

const defaultConfig = JSON.parse(fs.readFileSync(`${__dirname}/.swcrc`, 'utf-8'))

const config = {
  testEnvironment: "jsdom",
  transform: {
    '^.+\\.(t|j)sx?$': ['@swc/jest', {...defaultConfig}],
  },
  setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
};

module.exports = config;