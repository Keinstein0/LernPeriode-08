import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig, loadEnv } from 'vite';
import path from 'node:path';

export default defineConfig(({ mode }) => {
	// process.cwd() is the 'Frontend' folder. '..' goes one up.
	const envPath = path.resolve(process.cwd(), '..');
	
	// This manually loads the env file so we can verify it's working
	const env = loadEnv(mode, envPath, '');

	// --- DEBUG LOGS: Check your terminal when you run 'npm run dev' ---
	console.log('Checking for .env in:', envPath);
	console.log('Is PUBLIC_API_BASE_URL found?:', !!env.PUBLIC_API_BASE_URL);
	// -----------------------------------------------------------------

	return {
		plugins: [sveltekit()],
		envDir: envPath,
		server: {
			fs: {
				// Allow Vite to serve files from the parent directory
				allow: [envPath]
			}
		}
	};
});