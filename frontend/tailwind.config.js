/** @type {import('tailwindcss').Config} */
const PrimeUI = require("tailwindcss-primeui");

module.exports = {
  content: [
    "./apps/student-portal/src/**/*.{html,ts,scss}",
    "./apps/teacher-portal/src/**/*.{html,ts,scss}",
    "./libs/ui/src/**/*.{html,ts,scss}",
  ],
  theme: {
    extend: {},
  },
  plugins: [PrimeUI],
};
