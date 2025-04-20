module.exports = {
  preset: "jest-preset-angular",
  setupFilesAfterEnv: ["<rootDir>/setup-jest.ts"],
  testMatch: ["**/+(*.)+(spec).+(ts)"],
  transform: {
    "^.+\\.(ts|js|html)$": "ts-jest",
  },
  moduleFileExtensions: ["ts", "html", "js", "json"],
  coverageDirectory: "coverage",
};
