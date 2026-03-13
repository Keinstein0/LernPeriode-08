<script>
  import { page } from '$app/state';
  
  // Define your nav items for cleaner code
  const navItems = [
    { path: '/app/playlists', label: 'Playlists' },
    { path: '/app/search', label: 'Explore' },
    { path: '/app/account', label: 'You' }
  ];
</script>

<slot /> <nav class="hotbar">
  {#each navItems as item}
    <a 
      href={item.path} 
      class:active={page.url.pathname === item.path}
    >
      {item.label}
    </a>
  {/each}
</nav>

<style>
  .active {
    color: rgb(255, 0, 0);
    font-weight: bold;
    border-bottom: 2px solid rgb(255, 0, 0);
  }
</style>