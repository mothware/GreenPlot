/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        paper: {
          50:  '#faf8f3',
          100: '#f5f0e4',
          200: '#ede3cc',
        },
        leaf: {
          50:  '#f0f7f0',
          100: '#d4ebd4',
          500: '#3a7d3a',
          600: '#2d6b2d',
          700: '#1f5a1f',
          800: '#174517',
          900: '#0f2e0f',
        },
        tomato: {
          400: '#f06050',
          500: '#e84532',
          600: '#c93020',
        },
        soil: {
          300: '#b89b78',
          400: '#9a7f5a',
          500: '#7d6442',
          600: '#5e4a2c',
        },
        honey: {
          300: '#f5c842',
          400: '#e8b820',
          500: '#c99a00',
        },
      },
      fontFamily: {
        display: ['Fraunces', 'Georgia', 'serif'],
        sans: ['Instrument Sans', 'system-ui', 'sans-serif'],
        mono: ['JetBrains Mono', 'monospace'],
      },
    },
  },
  plugins: [],
}

