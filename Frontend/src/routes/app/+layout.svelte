<script lang="js">
  import { page } from '$app/state';
  import { onMount } from 'svelte';
  import Icon from './icon.svelte';
  
  /** @type {string | null} */
  let uuid = $state(null);

  let isSongFulscreen = false;

  onMount(() => {
      uuid = sessionStorage.getItem('uuid') || 'default';
  });

  let navItems = $derived([
    { path: '/app/playlists', label: 'Playlists', imgUrl: "playlist.svg", className: "icon" },
    { path: '/app/search', label: 'Explore', imgUrl: "explore.svg", className: "icon" },
    { 
      path: '/app/account', 
      label: 'You', 
      // The URL updates reactively once uuid is set in onMount
      imgUrl: uuid ? `https://api.dicebear.com/9.x/thumbs/svg?seed=${uuid}` : "", 
      className: "user" 
    }
  ]);

  $inspect(navItems)


  let { children } = $props();
  let yOffset = $state(0);
</script>

<div class="page-wrapper">
  <main class="content">
    {#if !isSongFulscreen}
      {@render children()}
    {:else}
      <p>testttttt</p>
    {/if}
  </main>

  {#if !isSongFulscreen}
    <button class="add-song-button"></button>
  {/if}
  

  <nav class="hotbar">

    {#each navItems as item}
      <a 
        href={item.path} 
        class="hotbar-item"
        class:active={page.url.pathname === item.path}
      >
        <Icon item={item} color="#1ed760" isActive = {page.url.pathname === item.path}/>
        <span class="label">{item.label}</span>
      </a>
    {/each}
  </nav>
</div>

<style>
  :global(body, html) {
    margin: 0;
    padding: 0;
    overflow: hidden;
  }

  @font-face{
    font-family: TangoSans;
    src: url('/Fonts/TangoSans.ttf');
  }

  .page-wrapper {
    display: flex;
    flex-direction: column;
    height: 100dvh;
    width: 100%;
    background-color: rgb(0, 0, 0);
  }

  .content {
    flex: 1;
    overflow-y: auto;
    padding: 1rem;
  }

  .hotbar {
    height: 65px;
    flex-shrink: 0;
    background: #080808;
    display: flex;
    justify-content: space-around;
    align-items: center;
    border-top: 4px solid #282828;
  }

  .hotbar-item {
    display: flex;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    color: white;
    gap: 4px;
    flex-direction: column; 
    font-size: 0.75rem;
    transition: all 0.2s ease;
    height: 100%;
    width: 100%;
    font-family: TangoSans;
  }

  @media (min-width: 600px) {
    .hotbar-item {
      flex-direction: row;
      font-size: 1rem;
      gap: 10px;
    }
  } 

  .add-song-button {
    position: absolute;
    bottom: 80px;
    right: 20px;

    width: 50px; 
    height: 50px;
    background-color: #1ed760; /* Spotify Green */
    border-radius: 50%;
    border: none;
    cursor: pointer;
    
    display: flex;
    align-items: center;
    justify-content: center;
    
    transition: 0.2s;
  }

  .add-song-button::after {
    content: "";
    width: 30px; 
    height: 30px;
    background-color: white;
    
    mask: url("/Images/plus.svg") no-repeat center;
    -webkit-mask: url("/Images/plus.svg") no-repeat center;
    mask-size: contain;
    -webkit-mask-size: contain;
  }

  .active {
    color: #1ed760;
    font-weight: bold;
  }

  :global(.user){
    border-radius: 1000px;
    outline: 1px solid #e0e0e0;
    outline-offset: 2px;
  }

  :global(.active-img) {
    outline: 2px solid #1ed760;
    outline-offset: 2px;
  }

  .add-song-button:hover{
    scale: 1.03;
    visibility: 90%;
  }


</style>