/**
 * GameHaven browser effects.
 *
 * Three independent pieces, each safe to call more than once and each with a
 * matching dispose so Blazor can tear them down on navigation:
 *   - hero:   a WebGL wireframe solid orbited by a particle field
 *   - tilt:   pointer-driven 3D rotation and specular sheen on the game cards
 *   - reveal: scroll-in transitions for [data-reveal] elements
 *
 * Every entry point degrades to a no-op when the browser reports reduced motion
 * or when WebGL is unavailable; the CSS ambient layers still carry the depth.
 */

const prefersReducedMotion = () =>
  window.matchMedia('(prefers-reduced-motion: reduce)').matches;

/**
 * Resolves once `element` has a non-zero box, or with null if it never gets one.
 *
 * Blazor calls into here from OnAfterRenderAsync, which can land before the
 * browser has laid the section out; reading clientWidth then yields 0. Bailing
 * out at that point is what silently left the hero flat, so wait for the box
 * instead of giving up on it.
 */
function waitForSize(element, timeoutMs = 3000) {
  const measure = () => ({ width: element.clientWidth, height: element.clientHeight });

  const initial = measure();
  if (initial.width > 0 && initial.height > 0) return Promise.resolve(initial);

  return new Promise((resolve) => {
    let settled = false;

    const finish = (value) => {
      if (settled) return;
      settled = true;
      observer.disconnect();
      clearTimeout(timer);
      resolve(value);
    };

    const observer = new ResizeObserver(() => {
      const size = measure();
      if (size.width > 0 && size.height > 0) finish(size);
    });

    const timer = setTimeout(() => finish(null), timeoutMs);

    observer.observe(element);
  });
}

/* ------------------------------------------------------------------ hero -- */

let hero = null;

export async function initHero(canvasId) {
  disposeHero();

  if (prefersReducedMotion()) return;

  const canvas = document.getElementById(canvasId);
  if (!canvas) return;

  const host = canvas.parentElement;
  if (!host) return;

  let THREE;
  try {
    THREE = await import('./three.module.min.js');
  } catch {
    return; // no 3D layer; the page is still complete without it
  }

  const size = await waitForSize(host);
  if (!size) return; // never laid out; nothing sensible to render into

  let width = size.width;
  let height = size.height;

  let renderer;
  try {
    renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true });
  } catch {
    return; // WebGL blocked or unsupported
  }

  // Pixel ratio first: setSize multiplies by it to size the backing store.
  renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
  renderer.setSize(width, height, false);

  const scene = new THREE.Scene();
  const camera = new THREE.PerspectiveCamera(70, width / height, 0.1, 100);
  camera.position.z = 6;

  // Core solid: two nested icosahedra turning at different rates read as a
  // single object with internal structure.
  const outerShell = new THREE.Mesh(
    new THREE.IcosahedronGeometry(2.35, 1),
    new THREE.MeshBasicMaterial({
      color: 0x3b82f6,
      wireframe: true,
      transparent: true,
      opacity: 0.26
    })
  );
  scene.add(outerShell);

  const innerCore = new THREE.Mesh(
    new THREE.IcosahedronGeometry(1.25, 0),
    new THREE.MeshBasicMaterial({
      color: 0x60a5fa,
      wireframe: true,
      transparent: true,
      opacity: 0.4
    })
  );
  scene.add(innerCore);

  // Particle halo. Positions are static; the whole cloud rotates as one, which
  // costs a single matrix update per frame instead of 600 buffer writes.
  const particleCount = 600;
  const positions = new Float32Array(particleCount * 3);
  for (let i = 0; i < particleCount; i++) {
    const radius = 2.9 + Math.random() * 5.4;
    const theta = Math.random() * Math.PI * 2;
    const phi = Math.acos(Math.random() * 2 - 1);

    positions[i * 3] = radius * Math.sin(phi) * Math.cos(theta);
    positions[i * 3 + 1] = radius * Math.sin(phi) * Math.sin(theta);
    positions[i * 3 + 2] = radius * Math.cos(phi);
  }

  const particleGeometry = new THREE.BufferGeometry();
  particleGeometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));

  const particleMaterial = new THREE.PointsMaterial({
    size: 0.03,
    color: 0x93c5fd,
    transparent: true,
    opacity: 0.5,
    blending: THREE.AdditiveBlending,
    depthWrite: false
  });

  const particles = new THREE.Points(particleGeometry, particleMaterial);
  scene.add(particles);

  let pointerX = 0;
  let pointerY = 0;
  let scrollY = window.scrollY;
  let frameId = null;
  let running = false;

  const onPointerMove = (event) => {
    const rect = host.getBoundingClientRect();
    pointerX = ((event.clientX - rect.left) / rect.width - 0.5) * 2;
    pointerY = ((event.clientY - rect.top) / rect.height - 0.5) * 2;
  };

  const onScroll = () => {
    scrollY = window.scrollY;
  };

  const onResize = () => {
    width = host.clientWidth;
    height = host.clientHeight;
    if (width === 0 || height === 0) return;
    camera.aspect = width / height;
    camera.updateProjectionMatrix();
    renderer.setSize(width, height, false);
  };

  const clock = new THREE.Clock();

  const render = () => {
    frameId = requestAnimationFrame(render);

    const elapsed = clock.getElapsedTime();

    outerShell.rotation.x = elapsed * 0.11 + pointerY * 0.25;
    outerShell.rotation.y = elapsed * 0.16 + pointerX * 0.35;

    innerCore.rotation.x = elapsed * -0.22;
    innerCore.rotation.y = elapsed * -0.3;
    innerCore.scale.setScalar(1 + Math.sin(elapsed * 1.1) * 0.06);

    particles.rotation.y = elapsed * 0.035;
    particles.rotation.x = pointerY * 0.12;

    // Parallax: the solid sinks slightly as the storefront scrolls past it.
    camera.position.y = -scrollY * 0.0016;
    camera.lookAt(0, 0, 0);

    renderer.render(scene, camera);
  };

  const start = () => {
    if (running) return;
    running = true;
    clock.start();
    render();
  };

  const stop = () => {
    if (!running) return;
    running = false;
    if (frameId) cancelAnimationFrame(frameId);
    frameId = null;
  };

  // Only burn frames while the hero is actually on screen and the tab is active.
  const observer = new IntersectionObserver(
    ([entry]) => (entry.isIntersecting ? start() : stop()),
    { threshold: 0.01 }
  );
  observer.observe(host);

  const onVisibilityChange = () => (document.hidden ? stop() : start());

  host.addEventListener('pointermove', onPointerMove, { passive: true });
  window.addEventListener('scroll', onScroll, { passive: true });
  window.addEventListener('resize', onResize, { passive: true });
  document.addEventListener('visibilitychange', onVisibilityChange);

  hero = {
    dispose() {
      stop();
      observer.disconnect();
      host.removeEventListener('pointermove', onPointerMove);
      window.removeEventListener('scroll', onScroll);
      window.removeEventListener('resize', onResize);
      document.removeEventListener('visibilitychange', onVisibilityChange);

      scene.clear();
      outerShell.geometry.dispose();
      outerShell.material.dispose();
      innerCore.geometry.dispose();
      innerCore.material.dispose();
      particleGeometry.dispose();
      particleMaterial.dispose();
      renderer.dispose();
    }
  };
}

export function disposeHero() {
  if (!hero) return;
  hero.dispose();
  hero = null;
}

/* ------------------------------------------------------------------ tilt -- */

let tilt = null;

const MAX_TILT_DEGREES = 7;

export function initTilt(containerSelector) {
  disposeTilt();

  if (prefersReducedMotion()) return;
  if (window.matchMedia('(hover: none)').matches) return; // touch: no pointer to follow

  const container = document.querySelector(containerSelector);
  if (!container) return;

  let activeCard = null;
  let pointerX = 0;
  let pointerY = 0;
  let frameId = null;

  const apply = () => {
    frameId = null;
    if (!activeCard) return;

    const rect = activeCard.getBoundingClientRect();
    const x = pointerX - rect.left;
    const y = pointerY - rect.top;

    const rotateX = ((y - rect.height / 2) / (rect.height / 2)) * -MAX_TILT_DEGREES;
    const rotateY = ((x - rect.width / 2) / (rect.width / 2)) * MAX_TILT_DEGREES;

    activeCard.style.transform =
      `rotateX(${rotateX.toFixed(2)}deg) rotateY(${rotateY.toFixed(2)}deg) translateY(-6px) scale(1.015)`;
    activeCard.style.setProperty('--card-mouse-x', `${x}px`);
    activeCard.style.setProperty('--card-mouse-y', `${y}px`);
  };

  const release = (card) => {
    if (!card) return;
    card.style.transform = '';
    card.style.transition = '';
    card.style.willChange = '';
    card.style.removeProperty('--card-mouse-x');
    card.style.removeProperty('--card-mouse-y');
  };

  const onPointerMove = (event) => {
    const card = event.target.closest('[data-tilt-card]');

    if (card !== activeCard) {
      release(activeCard);
      activeCard = card;
      if (activeCard) {
        activeCard.style.willChange = 'transform';
        // Spring only on the way in; per-frame updates must not be animated.
        activeCard.style.transition = 'transform 220ms cubic-bezier(0.34, 1.56, 0.64, 1)';
        setTimeout(() => {
          if (activeCard === card) card.style.transition = 'transform 120ms linear';
        }, 220);
      }
    }

    if (!activeCard) return;

    pointerX = event.clientX;
    pointerY = event.clientY;

    if (frameId === null) frameId = requestAnimationFrame(apply);
  };

  const onPointerLeave = () => {
    release(activeCard);
    activeCard = null;
  };

  container.addEventListener('pointermove', onPointerMove, { passive: true });
  container.addEventListener('pointerleave', onPointerLeave, { passive: true });

  tilt = {
    dispose() {
      container.removeEventListener('pointermove', onPointerMove);
      container.removeEventListener('pointerleave', onPointerLeave);
      if (frameId) cancelAnimationFrame(frameId);
      release(activeCard);
      activeCard = null;
    }
  };
}

export function disposeTilt() {
  if (!tilt) return;
  tilt.dispose();
  tilt = null;
}

/* ---------------------------------------------------------------- reveal -- */

let revealObserver = null;

export function initReveal() {
  disposeReveal();

  const targets = document.querySelectorAll('[data-reveal]:not(.is-revealed)');
  if (targets.length === 0) return;

  if (prefersReducedMotion() || !('IntersectionObserver' in window)) {
    return; // nothing to arm; the elements are already visible
  }

  // Arm the hidden state only now that the observer is guaranteed to exist.
  targets.forEach((el) => el.classList.add('reveal-armed'));

  revealObserver = new IntersectionObserver(
    (entries, observer) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) return;
        entry.target.classList.add('is-revealed');
        observer.unobserve(entry.target);
      });
    },
    { threshold: 0.12, rootMargin: '0px 0px -8% 0px' }
  );

  targets.forEach((el) => revealObserver.observe(el));
}

export function disposeReveal() {
  if (!revealObserver) return;
  revealObserver.disconnect();
  revealObserver = null;
}

/* ------------------------------------------------------------------ misc -- */

export function scrollToTop() {
  window.scrollTo({
    top: 0,
    behavior: prefersReducedMotion() ? 'auto' : 'smooth'
  });
}

export function scrollIntoView(elementId) {
  document.getElementById(elementId)?.scrollIntoView({
    behavior: prefersReducedMotion() ? 'auto' : 'smooth',
    block: 'start'
  });
}
