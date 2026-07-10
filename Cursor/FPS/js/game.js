import * as THREE from 'three';

// ── Constants ──────────────────────────────────────────────
const ARENA_SIZE = 40;
const WALL_HEIGHT = 6;
const PLAYER_SPEED = 12;
const PLAYER_ROT_SPEED = 2.5;
const BULLET_SPEED = 45;
const BULLET_DAMAGE = 15;
const FIRE_COOLDOWN = 0.35;
const BOT_SPEED = 8;
const BOT_ROT_SPEED = 1.8;
const BOT_FIRE_COOLDOWN = 0.8;
const BOT_DAMAGE = 10;
const MAX_HP = 100;
const BOT_SIGHT_RANGE = 35;
const BOT_SHOOT_RANGE = 30;

// ── Game State ─────────────────────────────────────────────
let scene, camera, renderer, clock;
let arena, enemy;
let bullets = [];
let particles = [];
let keys = {};
let gameState = 'menu'; // menu | playing | over
let playerHP = MAX_HP;
let enemyHP = MAX_HP;
let playerFireTimer = 0;
let botFireTimer = 0;
let botState = 'patrol';
let botPatrolTarget = new THREE.Vector3();
let botPatrolTimer = 0;
let muzzleFlash = null;

// ── DOM ────────────────────────────────────────────────────
const canvas = document.getElementById('game-canvas');
const startScreen = document.getElementById('start-screen');
const hud = document.getElementById('hud');
const gameOverScreen = document.getElementById('game-over');
const resultTitle = document.getElementById('result-title');
const resultMessage = document.getElementById('result-message');
const playerHPBar = document.getElementById('player-hp');
const enemyHPBar = document.getElementById('enemy-hp');

document.getElementById('start-btn').addEventListener('click', startGame);
document.getElementById('restart-btn').addEventListener('click', startGame);

window.addEventListener('keydown', (e) => {
  keys[e.code] = true;
  if (e.code === 'Space' && gameState === 'playing') {
    e.preventDefault();
  }
});
window.addEventListener('keyup', (e) => { keys[e.code] = false; });

// ── Init Three.js ──────────────────────────────────────────
function init() {
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0x0a0a12);
  scene.fog = new THREE.Fog(0x0a0a12, 20, 55);

  camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 100);
  camera.position.set(0, 1.7, ARENA_SIZE / 2 - 4);

  renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
  renderer.setSize(window.innerWidth, window.innerHeight);
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  renderer.shadowMap.enabled = true;
  renderer.shadowMap.type = THREE.PCFSoftShadowMap;

  clock = new THREE.Clock();

  // Lighting
  const ambient = new THREE.AmbientLight(0x334466, 0.6);
  scene.add(ambient);

  const sun = new THREE.DirectionalLight(0xffeedd, 1.2);
  sun.position.set(15, 25, 10);
  sun.castShadow = true;
  sun.shadow.mapSize.set(2048, 2048);
  sun.shadow.camera.near = 1;
  sun.shadow.camera.far = 60;
  sun.shadow.camera.left = -30;
  sun.shadow.camera.right = 30;
  sun.shadow.camera.top = 30;
  sun.shadow.camera.bottom = -30;
  scene.add(sun);

  const rimLight = new THREE.PointLight(0x4466ff, 0.8, 50);
  rimLight.position.set(0, 8, 0);
  scene.add(rimLight);

  buildArena();
  buildEnemy();
  buildWeapon();

  window.addEventListener('resize', onResize);
  animate();
}

// ── Arena ──────────────────────────────────────────────────
function buildArena() {
  arena = new THREE.Group();
  scene.add(arena);

  const floorGeo = new THREE.PlaneGeometry(ARENA_SIZE, ARENA_SIZE, 20, 20);
  const floorMat = new THREE.MeshStandardMaterial({
    color: 0x1a1a2e,
    roughness: 0.8,
    metalness: 0.3,
  });
  const floor = new THREE.Mesh(floorGeo, floorMat);
  floor.rotation.x = -Math.PI / 2;
  floor.receiveShadow = true;
  arena.add(floor);

  // Grid lines on floor
  const gridHelper = new THREE.GridHelper(ARENA_SIZE, 20, 0x334466, 0x222244);
  gridHelper.position.y = 0.01;
  arena.add(gridHelper);

  const wallMat = new THREE.MeshStandardMaterial({
    color: 0x2a2a44,
    roughness: 0.6,
    metalness: 0.4,
  });

  const half = ARENA_SIZE / 2;
  const walls = [
    { pos: [0, WALL_HEIGHT / 2, -half], size: [ARENA_SIZE, WALL_HEIGHT, 0.5] },
    { pos: [0, WALL_HEIGHT / 2, half], size: [ARENA_SIZE, WALL_HEIGHT, 0.5] },
    { pos: [-half, WALL_HEIGHT / 2, 0], size: [0.5, WALL_HEIGHT, ARENA_SIZE] },
    { pos: [half, WALL_HEIGHT / 2, 0], size: [0.5, WALL_HEIGHT, ARENA_SIZE] },
  ];

  walls.forEach(({ pos, size }) => {
    const wall = new THREE.Mesh(new THREE.BoxGeometry(...size), wallMat);
    wall.position.set(...pos);
    wall.castShadow = true;
    wall.receiveShadow = true;
    arena.add(wall);
  });

  // Decorative pillars
  const pillarMat = new THREE.MeshStandardMaterial({ color: 0x3a3a66, metalness: 0.6, roughness: 0.4 });
  const pillarPositions = [
    [-12, 0, -12], [12, 0, -12], [-12, 0, 12], [12, 0, 12],
    [-12, 0, 0], [12, 0, 0], [0, 0, -12], [0, 0, 12],
  ];
  pillarPositions.forEach(([x, , z]) => {
    const pillar = new THREE.Mesh(new THREE.CylinderGeometry(0.8, 0.8, WALL_HEIGHT, 8), pillarMat);
    pillar.position.set(x, WALL_HEIGHT / 2, z);
    pillar.castShadow = true;
    pillar.receiveShadow = true;
    arena.add(pillar);
  });

  // Ceiling glow strips
  const stripMat = new THREE.MeshBasicMaterial({ color: 0x4466aa });
  for (let i = -1; i <= 1; i++) {
    const strip = new THREE.Mesh(new THREE.BoxGeometry(ARENA_SIZE - 2, 0.1, 0.3), stripMat);
    strip.position.set(0, WALL_HEIGHT - 0.2, i * 10);
    arena.add(strip);
  }
}

// ── Enemy Bot ──────────────────────────────────────────────
function buildEnemy() {
  enemy = new THREE.Group();

  const bodyMat = new THREE.MeshStandardMaterial({ color: 0xcc2244, metalness: 0.5, roughness: 0.4 });
  const accentMat = new THREE.MeshStandardMaterial({ color: 0xff4466, emissive: 0xff2244, emissiveIntensity: 0.3 });
  const darkMat = new THREE.MeshStandardMaterial({ color: 0x111122, metalness: 0.7, roughness: 0.3 });

  const body = new THREE.Mesh(new THREE.BoxGeometry(1.2, 1.6, 0.8), bodyMat);
  body.position.y = 1.2;
  body.castShadow = true;
  enemy.add(body);

  const head = new THREE.Mesh(new THREE.BoxGeometry(0.8, 0.7, 0.7), accentMat);
  head.position.y = 2.3;
  head.castShadow = true;
  enemy.add(head);

  const eyeGeo = new THREE.SphereGeometry(0.1, 8, 8);
  const eyeMat = new THREE.MeshBasicMaterial({ color: 0xff0000 });
  [-0.2, 0.2].forEach((x) => {
    const eye = new THREE.Mesh(eyeGeo, eyeMat);
    eye.position.set(x, 2.35, 0.36);
    enemy.add(eye);
  });

  const armGeo = new THREE.BoxGeometry(0.3, 1.0, 0.3);
  const leftArm = new THREE.Mesh(armGeo, darkMat);
  leftArm.position.set(-0.85, 1.2, 0);
  enemy.add(leftArm);

  const rightArm = new THREE.Mesh(armGeo, darkMat);
  rightArm.position.set(0.85, 1.2, 0.3);
  rightArm.rotation.x = -0.5;
  enemy.add(rightArm);

  const gun = new THREE.Mesh(new THREE.BoxGeometry(0.15, 0.15, 0.6), accentMat);
  gun.position.set(0.85, 1.0, 0.7);
  enemy.add(gun);

  const legGeo = new THREE.BoxGeometry(0.35, 0.8, 0.35);
  [-0.3, 0.3].forEach((x) => {
    const leg = new THREE.Mesh(legGeo, darkMat);
    leg.position.set(x, 0.4, 0);
    enemy.add(leg);
  });

  enemy.position.set(0, 0, -ARENA_SIZE / 2 + 4);
  scene.add(enemy);
  pickBotPatrolTarget();
}

// ── Weapon (visible in FPS) ─────────────────────────────────
function buildWeapon() {
  const weaponGroup = new THREE.Group();

  const gunBody = new THREE.Mesh(
    new THREE.BoxGeometry(0.08, 0.12, 0.5),
    new THREE.MeshStandardMaterial({ color: 0x333344, metalness: 0.8, roughness: 0.3 })
  );
  gunBody.position.set(0.25, -0.2, -0.4);
  weaponGroup.add(gunBody);

  const barrel = new THREE.Mesh(
    new THREE.CylinderGeometry(0.025, 0.025, 0.3, 8),
    new THREE.MeshStandardMaterial({ color: 0x222233, metalness: 0.9, roughness: 0.2 })
  );
  barrel.rotation.x = Math.PI / 2;
  barrel.position.set(0.25, -0.18, -0.7);
  weaponGroup.add(barrel);

  muzzleFlash = new THREE.PointLight(0xffaa44, 0, 3);
  muzzleFlash.position.set(0.25, -0.18, -0.85);
  weaponGroup.add(muzzleFlash);

  camera.add(weaponGroup);
  scene.add(camera);
}

// ── Game Flow ──────────────────────────────────────────────
function startGame() {
  playerHP = MAX_HP;
  enemyHP = MAX_HP;
  playerFireTimer = 0;
  botFireTimer = 0;
  botState = 'patrol';
  botPatrolTimer = 0;

  bullets.forEach((b) => scene.remove(b.mesh));
  bullets = [];
  particles.forEach((p) => scene.remove(p.mesh));
  particles = [];

  camera.position.set(0, 1.7, ARENA_SIZE / 2 - 4);
  camera.rotation.set(0, 0, 0);

  enemy.position.set(0, 0, -ARENA_SIZE / 2 + 4);
  enemy.rotation.y = 0;
  pickBotPatrolTarget();

  updateHUD();
  startScreen.classList.add('hidden');
  gameOverScreen.classList.add('hidden');
  hud.classList.remove('hidden');
  gameState = 'playing';
}

function endGame(won) {
  gameState = 'over';
  hud.classList.add('hidden');
  gameOverScreen.classList.remove('hidden');
  resultTitle.textContent = won ? '勝利!' : '敗北...';
  resultTitle.className = won ? 'victory' : 'defeat';
  resultMessage.textContent = won
    ? 'コンピュータ対戦相手を撃破しました!'
    : 'コンピュータ対戦相手に倒されました。';
}

function updateHUD() {
  playerHPBar.style.width = `${(playerHP / MAX_HP) * 100}%`;
  enemyHPBar.style.width = `${(enemyHP / MAX_HP) * 100}%`;
}

// ── Player Controls ────────────────────────────────────────
function updatePlayer(dt) {
  if (keys['ArrowLeft']) camera.rotation.y += PLAYER_ROT_SPEED * dt;
  if (keys['ArrowRight']) camera.rotation.y -= PLAYER_ROT_SPEED * dt;

  const forward = new THREE.Vector3(0, 0, -1).applyQuaternion(camera.quaternion);
  forward.y = 0;
  forward.normalize();

  const move = new THREE.Vector3();
  if (keys['ArrowUp']) move.add(forward);
  if (keys['ArrowDown']) move.sub(forward);

  if (move.lengthSq() > 0) {
    move.normalize().multiplyScalar(PLAYER_SPEED * dt);
    camera.position.add(move);
  }

  clampToArena(camera.position, 1.0);

  playerFireTimer -= dt;
  if (keys['Space'] && playerFireTimer <= 0) {
    fireBullet(camera.position.clone(), camera.getWorldDirection(new THREE.Vector3()), 'player');
    playerFireTimer = FIRE_COOLDOWN;
    flashMuzzle();
  }
}

function flashMuzzle() {
  muzzleFlash.intensity = 3;
  setTimeout(() => { muzzleFlash.intensity = 0; }, 50);
}

// ── Bot AI ─────────────────────────────────────────────────
function updateBot(dt) {
  const distToPlayer = enemy.position.distanceTo(camera.position);
  const canSeePlayer = distToPlayer < BOT_SIGHT_RANGE && hasLineOfSight(enemy.position, camera.position);

  if (canSeePlayer) {
    botState = 'combat';
  } else if (botState === 'combat' && distToPlayer > BOT_SIGHT_RANGE + 5) {
    botState = 'patrol';
  }

  if (botState === 'combat') {
    // Face player
    const dir = new THREE.Vector3().subVectors(camera.position, enemy.position);
    dir.y = 0;
    const targetAngle = Math.atan2(dir.x, dir.z);
    enemy.rotation.y = lerpAngle(enemy.rotation.y, targetAngle, BOT_ROT_SPEED * dt);

    // Move toward player if far
    if (distToPlayer > 8) {
      const moveDir = dir.normalize();
      enemy.position.add(moveDir.multiplyScalar(BOT_SPEED * dt));
      clampToArena(enemy.position, 1.0);
    }

    // Shoot
    botFireTimer -= dt;
    if (botFireTimer <= 0 && distToPlayer < BOT_SHOOT_RANGE && canSeePlayer) {
      const shootDir = new THREE.Vector3().subVectors(camera.position, enemy.position);
      shootDir.y += 0.3;
      shootDir.normalize();
      const muzzlePos = enemy.position.clone();
      muzzlePos.y = 1.5;
      muzzlePos.add(shootDir.clone().multiplyScalar(1.0));
      fireBullet(muzzlePos, shootDir, 'bot');
      botFireTimer = BOT_FIRE_COOLDOWN;
    }
  } else {
    // Patrol
    botPatrolTimer -= dt;
    const toTarget = new THREE.Vector3().subVectors(botPatrolTarget, enemy.position);
    toTarget.y = 0;
    const dist = toTarget.length();

    if (dist < 1.5 || botPatrolTimer <= 0) {
      pickBotPatrolTarget();
    } else {
      toTarget.normalize();
      const targetAngle = Math.atan2(toTarget.x, toTarget.z);
      enemy.rotation.y = lerpAngle(enemy.rotation.y, targetAngle, BOT_ROT_SPEED * dt * 0.5);
      enemy.position.add(toTarget.multiplyScalar(BOT_SPEED * 0.6 * dt));
      clampToArena(enemy.position, 1.0);
    }
  }
}

function pickBotPatrolTarget() {
  const margin = 4;
  const half = ARENA_SIZE / 2 - margin;
  botPatrolTarget.set(
    (Math.random() - 0.5) * half * 2,
    0,
    (Math.random() - 0.5) * half * 2
  );
  botPatrolTimer = 3 + Math.random() * 4;
}

function hasLineOfSight(from, to) {
  const dir = new THREE.Vector3().subVectors(to, from);
  const dist = dir.length();
  dir.normalize();

  const raycaster = new THREE.Raycaster(
    new THREE.Vector3(from.x, 1.5, from.z),
    dir,
    0,
    dist
  );

  const obstacles = [];
  arena.children.forEach((child) => {
    if (child.geometry && child.geometry.type !== 'PlaneGeometry' && child.geometry.type !== 'GridHelper') {
      obstacles.push(child);
    }
  });

  const hits = raycaster.intersectObjects(obstacles, false);
  return hits.length === 0;
}

// ── Bullets ────────────────────────────────────────────────
function fireBullet(origin, direction, owner) {
  const geo = new THREE.SphereGeometry(0.08, 6, 6);
  const mat = new THREE.MeshBasicMaterial({
    color: owner === 'player' ? 0x44aaff : 0xff4444,
  });
  const mesh = new THREE.Mesh(geo, mat);
  mesh.position.copy(origin);
  scene.add(mesh);

  const glow = new THREE.PointLight(owner === 'player' ? 0x44aaff : 0xff4444, 0.5, 2);
  mesh.add(glow);

  bullets.push({
    mesh,
    velocity: direction.clone().normalize().multiplyScalar(BULLET_SPEED),
    owner,
    life: 3,
  });
}

function updateBullets(dt) {
  for (let i = bullets.length - 1; i >= 0; i--) {
    const b = bullets[i];
    b.mesh.position.add(b.velocity.clone().multiplyScalar(dt));
    b.life -= dt;

    if (b.life <= 0) {
      scene.remove(b.mesh);
      bullets.splice(i, 1);
      continue;
    }

    // Wall collision
    const half = ARENA_SIZE / 2 - 0.5;
    const p = b.mesh.position;
    if (Math.abs(p.x) > half || Math.abs(p.z) > half || p.y < 0 || p.y > WALL_HEIGHT) {
      spawnHitEffect(p.clone(), b.owner === 'player' ? 0x44aaff : 0xff4444);
      scene.remove(b.mesh);
      bullets.splice(i, 1);
      continue;
    }

    // Pillar collision
    if (checkPillarCollision(p)) {
      spawnHitEffect(p.clone(), b.owner === 'player' ? 0x44aaff : 0xff4444);
      scene.remove(b.mesh);
      bullets.splice(i, 1);
      continue;
    }

    // Hit detection
    if (b.owner === 'player') {
      const enemyBox = new THREE.Box3().setFromObject(enemy);
      enemyBox.expandByScalar(0.3);
      if (enemyBox.containsPoint(p)) {
        enemyHP = Math.max(0, enemyHP - BULLET_DAMAGE);
        updateHUD();
        spawnHitEffect(p.clone(), 0xff4466);
        scene.remove(b.mesh);
        bullets.splice(i, 1);
        flashEnemy();
        if (enemyHP <= 0) endGame(true);
      }
    } else {
      const playerPos = camera.position;
      if (p.distanceTo(playerPos) < 1.0) {
        playerHP = Math.max(0, playerHP - BOT_DAMAGE);
        updateHUD();
        spawnHitEffect(p.clone(), 0x44aaff);
        scene.remove(b.mesh);
        bullets.splice(i, 1);
        flashScreen();
        if (playerHP <= 0) endGame(false);
      }
    }
  }
}

function checkPillarCollision(pos) {
  const pillarPositions = [
    [-12, -12], [12, -12], [-12, 12], [12, 12],
    [-12, 0], [12, 0], [0, -12], [0, 12],
  ];
  return pillarPositions.some(([x, z]) => {
    const dx = pos.x - x;
    const dz = pos.z - z;
    return dx * dx + dz * dz < 1.5 * 1.5;
  });
}

function flashEnemy() {
  enemy.children.forEach((child) => {
    if (child.material && child.material.emissive) {
      const orig = child.material.emissiveIntensity;
      child.material.emissiveIntensity = 1.5;
      setTimeout(() => { child.material.emissiveIntensity = orig; }, 100);
    }
  });
}

function flashScreen() {
  const flash = document.createElement('div');
  flash.style.cssText = 'position:fixed;inset:0;background:rgba(255,0,0,0.3);z-index:20;pointer-events:none;';
  document.body.appendChild(flash);
  setTimeout(() => flash.remove(), 150);
}

// ── Particles ──────────────────────────────────────────────
function spawnHitEffect(pos, color) {
  for (let i = 0; i < 8; i++) {
    const geo = new THREE.SphereGeometry(0.05, 4, 4);
    const mat = new THREE.MeshBasicMaterial({ color });
    const mesh = new THREE.Mesh(geo, mat);
    mesh.position.copy(pos);
    scene.add(mesh);

    const vel = new THREE.Vector3(
      (Math.random() - 0.5) * 6,
      Math.random() * 4,
      (Math.random() - 0.5) * 6
    );
    particles.push({ mesh, velocity: vel, life: 0.5 });
  }
}

function updateParticles(dt) {
  for (let i = particles.length - 1; i >= 0; i--) {
    const p = particles[i];
    p.velocity.y -= 10 * dt;
    p.mesh.position.add(p.velocity.clone().multiplyScalar(dt));
    p.life -= dt;
    p.mesh.material.opacity = Math.max(0, p.life * 2);
    if (p.life <= 0) {
      scene.remove(p.mesh);
      particles.splice(i, 1);
    }
  }
}

// ── Utilities ──────────────────────────────────────────────
function clampToArena(pos, margin) {
  const half = ARENA_SIZE / 2 - margin;
  pos.x = THREE.MathUtils.clamp(pos.x, -half, half);
  pos.z = THREE.MathUtils.clamp(pos.z, -half, half);
}

function lerpAngle(a, b, t) {
  let diff = b - a;
  while (diff > Math.PI) diff -= Math.PI * 2;
  while (diff < -Math.PI) diff += Math.PI * 2;
  return a + diff * Math.min(t, 1);
}

function onResize() {
  camera.aspect = window.innerWidth / window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(window.innerWidth, window.innerHeight);
}

// ── Main Loop ──────────────────────────────────────────────
function animate() {
  requestAnimationFrame(animate);
  const dt = Math.min(clock.getDelta(), 0.05);

  if (gameState === 'playing') {
    updatePlayer(dt);
    updateBot(dt);
    updateBullets(dt);
    updateParticles(dt);
  }

  renderer.render(scene, camera);
}

init();
