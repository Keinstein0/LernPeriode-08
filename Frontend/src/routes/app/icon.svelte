<script>
  export let item;
  export let color = "black";
  export let isActive = false;

  const { imgUrl, label, className: classMain } = item;

  const isExternal = imgUrl.startsWith('http');
  const src = isExternal ? imgUrl : `/Images/${imgUrl}`;

  $: isSvg = imgUrl.endsWith('.svg');
</script>

{#if isSvg}
  <div 
    role="img"
    aria-label={label}
    class="{classMain} {isActive ? 'active-img' : ''}"
    style="
      background-color: {isActive ? color : "white"}; 
      mask: url({src}) no-repeat center;
      -webkit-mask: url({src}) no-repeat center;
      mask-size: contain;
      -webkit-mask-size: contain;
      width: 24px; 
      height: 24px;
      display: inline-block;
      flex-shrink: 0;
    "
  ></div>
{:else}
  <img 
    {src} 
    alt={label} 
    class="{classMain} {isActive ? 'active-img' : ''}"
    style="width: 24px; height: 24px; flex-shrink: 0;" 
  />
{/if}
