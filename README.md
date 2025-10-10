<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <title>FIH — README</title>
  <meta name="viewport" content="width=device-width,initial-scale=1" />
  <style>
    :root{
      --bg:#071228;
      --card:#09283b;
      --accent:#2fd4c8;
      --muted:#9fb6c6;
      --glass: rgba(255,255,255,0.04);
      font-family: Inter, ui-sans-serif, system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial;
    }
    body{
      margin:0;
      background: linear-gradient(180deg,#021424 0%, #071228 60%);
      color:#e6f0f2;
      -webkit-font-smoothing:antialiased;
    }
    .wrap{
      max-width:900px;
      margin:36px auto;
      padding:28px;
      background: linear-gradient(180deg, rgba(255,255,255,0.02), rgba(255,255,255,0.01));
      border-radius:16px;
      box-shadow: 0 6px 30px rgba(1,10,18,0.6);
      border: 1px solid rgba(255,255,255,0.03);
    }
    header h1{
      margin:0 0 6px 0;
      font-size:clamp(28px,4vw,40px);
      letter-spacing: -0.02em;
    }
    header p.lead{
      margin:0 0 22px 0;
      color:var(--muted);
    }
    .badge{
      display:inline-block;
      padding:6px 10px;
      border-radius:999px;
      background:linear-gradient(90deg, rgba(47,212,200,0.08), rgba(47,212,200,0.03));
      color:var(--accent);
      font-weight:600;
      font-size:13px;
      border:1px solid rgba(47,212,200,0.08);
    }
    section{margin:20px 0;}
    h2{
      margin:0 0 8px 0;
      font-size:18px;
      color:#dff7f4;
    }
    p{color: #cfe9ec; line-height:1.55; margin:8px 0;}
    ul{margin:10px 0 0 20px;}
    code{background:var(--glass); padding:4px 8px; border-radius:6px; color:var(--accent); font-weight:600;}
    .feature-grid{
      display:grid;
      grid-template-columns:repeat(auto-fit,minmax(220px,1fr));
      gap:12px;
      margin-top:12px;
    }
    .card{
      background: linear-gradient(180deg, rgba(255,255,255,0.01), rgba(255,255,255,0.015));
      border-radius:12px;
      padding:12px;
      border:1px solid rgba(255,255,255,0.03);
      box-shadow: 0 6px 18px rgba(1,8,12,0.5);
    }
    .tip{color:#b5f0e9; font-style:italic;}
    footer{margin-top:22px; color:var(--muted); font-size:13px;}
    .highlight{color:var(--accent); font-weight:700;}
  </style>
</head>
<body>
  <div class="wrap" role="document" aria-label="FIH game readme">
    <header>
      <div class="badge">Mobile • Prototype</div>
      <h1>FIH — An intimate mobile fishing adventure</h1>
      <p class="lead">Cast your line, reel in the extraordinary. FIH is a cozy, immersive mobile game about catching unique fishes, cooking them into mouthwatering dishes, and selling your culinary creations to unlock rarer rods, exotic bait, and secret fishing grounds.</p>
    </header>

    <section>
      <h2>✨ Elevator pitch</h2>
      <p>FIH puts players in a small pocket world on their phone where every cast feels meaningful. Each fish has personality, every recipe tells a story, and every sale is a step toward gear that changes how you play. It’s calm to start, surprising to master — a mobile loop of discovery, collection, and progression.</p>
    </section>

    <section>
      <h2>🎮 Core gameplay loop</h2>
      <ul>
        <li><strong>Explore:</strong> Choose from daily fishing spots (dock, river bend, moonlit lagoon).</li>
        <li><strong>Catch:</strong> Use timing and the right bait to hook unique fish species — some only appear under specific weather or time windows.</li>
        <li><strong>Cook:</strong> Try recipes in your mobile kitchen — ingredients and cooking technique affect dish quality.</li>
        <li><strong>Sell & Upgrade:</strong> Sell dishes at your stall to earn coins and reputation. Unlock new rods, baits, and cooking tools that change the catching and crafting experience.</li>
      </ul>
      <p class="tip">Tip: Rare rods don’t just increase catch rate — they can reveal hidden behaviors in fish, like cooperative schooling or nocturnal feeding.</p>
    </section>

    <section>
      <h2>🌊 Features</h2>
      <div class="feature-grid">
        <div class="card">
          <h3>Unique fish roster</h3>
          <p>Dozens of hand-crafted fish with lore — from the luminescent <span class="highlight">Mooncarp</span> to the temperamental <span class="highlight">Spinebarb</span>.</p>
        </div>

        <div class="card">
          <h3>Mobile-first controls</h3>
          <p>Simple tap-and-swipe fishing with tactile feedback, one-thumb cooking minigames, and gesture-based selling for a satisfying on-the-go experience.</p>
        </div>

        <div class="card">
          <h3>Cooking & recipes</h3>
          <p>Combine fish, herbs, and techniques to create dishes of varying rarity. High-quality dishes bring better rewards and reputation boosts.</p>
        </div>

        <div class="card">
          <h3>Progression & discovery</h3>
          <p>Unlock rods and bait trees that radically alter playstyle. Some gear grants access to secret areas and hidden fish.</p>
        </div>
      </div>
    </section>

    <section>
      <h2>🐟 Example fish — a glimpse</h2>
      <ul>
        <li><strong>Mooncarp</strong> — glows faintly; appears only at night near kelp beds. Great for luminous stews.</li>
        <li><strong>Spinebarb</strong> — feisty, requires precise reeling to avoid snapped lines. Yields spicy skewers.</li>
        <li><strong>Glassfin</strong> — translucent; can only be caught with fragile tackle. Fetches high funds when poached ethically into delicate sashimi.</li>
        <li><strong>Fogling</strong> — a rare catch that appears during morning fog; an ingredient for signature dishes.</li>
      </ul>
      <p class="tip">Every fish has a <em>preferred</em> cooking method. Discovering those yields bonus reputation.</p>
    </section>

    <section>
      <h2>📱 Controls (mobile only)</h2>
      <ul>
        <li><strong>Cast:</strong> Swipe upward to control cast power and angle.</li>
        <li><strong>Bite:</strong> Tap when the line twitches to set the hook.</li>
        <li><strong>Reel:</strong> Short, rhythmic swipes to apply steady tension; long hold to perform a strong pull (risk of snap).</li>
        <li><strong>Cook:</strong> Multi-touch gestures and timed taps for minigame phases (searing, seasoning, plating).</li>
        <li><strong>Sell:</strong> Drag a finished dish onto a stall slot to present — better presentation = higher price.</li>
      </ul>
    </section>

    <section>
      <h2>🔧 Progression & systems</h2>
      <p>Players earn two main currencies:</p>
      <ul>
        <li><strong>Coins</strong> — used to buy rods, bait, kitchen upgrades.</li>
        <li><strong>Reputation</strong> — unlocks secret recipes, new vendors, and premium fishing grounds.</li>
      </ul>
      <p>Rods are the primary unlockable: tiered from <code>Rusty Rod</code> to <code>Celestial Troller</code>. Each rod changes cast distance, sensitivity, and special abilities (magnetize, echo-lure, etc.).</p>
    </section>

    <section>
      <h2>🎨 Visuals & sound</h2>
      <p>FIH embraces a warm, painterly aesthetic with hand-animated fish and subtle weather effects. Sound design focuses on soothing ambient water sounds, satisfying foley for reeling, and delightful sizzle cues while cooking.</p>
    </section>

    <section>
      <h2>🚀 Getting started — install & test</h2>
      <p>This project is targeted to mobile platforms (Android & iOS). Minimum tech notes:</p>
      <ul>
        <li>Engine: Unity / Godot / preferred mobile engine (choose one) — optimized single-scene flow for rapid iteration.</li>
        <li>Orientation: Portrait-first design (responsive to most aspect ratios).</li>
        <li>Input: Touch-only input pipeline; optional haptics for supported devices.</li>
      </ul>
      <p>Example quick-run (Unity CLI style):</p>
      <pre><code>git clone &lt;repo&gt; && open in Unity or run mobile build targets</code></pre>
      <p class="tip">Keep device throttling disabled during playtests to maintain consistent frame timings for fishing inputs.</p>
    </section>

    <section>
      <h2>🤝 Contributing</h2>
      <p>We’re building a calm, collectible experience. Contributions welcome: fish concepts, recipes, minigame tuning, accessibility improvements (large-tap targets, color-blind palettes), and localization.</p>
      <ul>
        <li>Open a PR with a one-paragraph design and mockup for new fish or recipe.</li>
        <li>Label performance-critical changes and include mobile test results.</li>
      </ul>
    </section>

    <section>
      <h2>📜 License & credits</h2>
      <p>FIH is currently under <strong>MIT</strong> for code and a permissive license for art placeholders. Replace placeholder assets with your own or licensed assets before publishing.</p>
      <p>Initial design & concept by the FIH studio — small dev team, big sea dreams.</p>
    </section>

    <footer>
      <p>If you’re ready to go deeper: try concepting a signature fish that ties to a local legend, or design a cooking minigame that rewards subtle timing. Happy casting — may your lines always find wonder.</p>
      <p style="margin-top:12px">— The FIH Crew</p>
    </footer>
  </div>
</body>
</html>
