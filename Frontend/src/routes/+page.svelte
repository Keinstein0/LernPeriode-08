<script lang="ts">
    import { onMount } from 'svelte';

    let source: string = "Loading...";
    let error: string | null = null;


    const API_URL = "http://localhost:5000/WeatherForecast/idk"; 

    onMount(async () => {
        try {
            const response = await fetch(API_URL);
            if (!response.ok) throw new Error('Network response was not ok');
            
            // Assuming the backend returns a plain string or an object { "source": "..." }
            const data = await response.text();
            source = data; 
        } catch (err) {
            console.error("Fetch error:", err);
            error = "Failed to load data";
            source = "Error";
        }
    });
</script>

<style>
    h1 {
        font-family: Arial, Helvetica, sans-serif;
        color: #333;
    }
    .error {
        color: crimson;
    }
</style>

{#if error}
    <p class="error">{error}</p>
{:else}
    <h1>Hello World from {source}</h1>
{/if}