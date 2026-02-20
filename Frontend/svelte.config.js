import adapter from '@sveltejs/adapter-static'; // Change this line
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

/** @type {import('@sveltejs/kit').Config} */
const config = {
    preprocess: vitePreprocess(),
    kit: {
        // adapter-static will put everything in the 'build' folder
        adapter: adapter({
            pages: 'build',
            assets: 'build',
            fallback: 'index.html', // Use 'index.html' if you want a true SPA
            precompress: false,
            strict: true
        })
    }
};

export default config;