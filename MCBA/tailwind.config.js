/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        '!**/{bin,obj,node_modules}/**',
        '**/*.{cshtml,html}',
    ],
    plugins: [require('@tailwindcss/forms'), require("@tailwindcss/typography"), require("daisyui")],
    // Themes can be chosen from here https://daisyui.com/docs/themes/
    daisyui: {
        themes: ["emerald"]
    }
}
