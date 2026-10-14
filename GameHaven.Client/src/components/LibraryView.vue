<script setup lang="ts">
import { ref } from 'vue'
import type { Game, OwnedGame } from '../types'

const props = defineProps<{
  ownedEntries: OwnedGame[]
  catalog: Game[]
}>()

const launchingId = ref<string | null>(null)
const runningId = ref<string | null>(null)

const getGameDetails = (gameId: string): Game | undefined => {
  return props.catalog.find((g) => g.id === gameId)
}

const handleLaunch = (gameId: string) => {
  launchingId.value = gameId
  setTimeout(() => {
    launchingId.value = null
    runningId.value = gameId
  }, 1200)
}

const handleStop = () => {
  runningId.value = null
}
</script>

<template>
  <div class="library-view">
    <div class="library-header">
      <div>
        <h2 class="library-title">My Game Library</h2>
        <p class="library-subtitle">All purchased titles linked to your account.</p>
      </div>
      <div class="library-stats font-mono">
        <span class="stat-item"><strong>{{ ownedEntries.length }}</strong> Games Owned</span>
      </div>
    </div>

    <div v-if="ownedEntries.length === 0" class="empty-library">
      <div class="empty-art">🎮</div>
      <h3>Your library is empty</h3>
      <p>Browse the store to acquire indie games and expand your collection.</p>
    </div>

    <div v-else class="library-grid">
      <div v-for="entry in ownedEntries" :key="entry.gameId" class="library-card">
        <div class="card-cover">
          <img
            v-if="getGameDetails(entry.gameId)"
            :src="getGameDetails(entry.gameId)?.coverImageUrl"
            :alt="getGameDetails(entry.gameId)?.title"
            class="cover-img"
          />
          <div class="cover-overlay"></div>
          <span class="status-badge font-mono" v-if="runningId === entry.gameId">RUNNING</span>
        </div>

        <div class="card-info">
          <h3 class="game-name">{{ getGameDetails(entry.gameId)?.title || 'Indie Game Title' }}</h3>
          <span class="game-genre">{{ getGameDetails(entry.gameId)?.genre || 'Indie' }}</span>

          <div class="game-meta font-mono">
            <span>Playtime: {{ Math.floor(entry.playTimeMinutes / 60) }}h {{ entry.playTimeMinutes % 60 }}m</span>
            <span>Acquired: {{ new Date(entry.purchaseDate).toLocaleDateString() }}</span>
          </div>

          <div class="action-row">
            <button
              v-if="runningId === entry.gameId"
              class="btn-secondary stop-btn"
              @click="handleStop"
            >
              Close Game
            </button>
            <button
              v-else
              class="btn-primary play-btn"
              :disabled="launchingId === entry.gameId"
              @click="handleLaunch(entry.gameId)"
            >
              <span v-if="launchingId === entry.gameId">Launching...</span>
              <span v-else>▶ Play</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.library-view {
  padding-bottom: 4rem;
}

.library-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  margin: 2rem 0;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--line-dim);
}

.library-title {
  font-size: 1.75rem;
  font-weight: 800;
  color: #fff;
}

.library-subtitle {
  font-size: 0.875rem;
  color: var(--text-muted);
}

.stat-item {
  background: var(--bg-elevated);
  padding: 0.4rem 0.8rem;
  border-radius: var(--radius-md);
  border: 1px solid var(--line-dim);
  font-size: 0.8125rem;
  color: var(--text-muted);
}

.stat-item strong {
  color: var(--accent-cyan);
}

.empty-library {
  text-align: center;
  padding: 5rem 1rem;
  color: var(--text-dim);
}

.empty-art {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-library h3 {
  font-size: 1.25rem;
  color: #fff;
  margin-bottom: 0.5rem;
}

.library-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
}

.library-card {
  background: var(--bg-surface);
  border: 1px solid var(--line-dim);
  border-radius: var(--radius-lg);
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.card-cover {
  position: relative;
  width: 100%;
  aspect-ratio: 16 / 9;
  background: var(--bg-elevated);
}

.cover-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.cover-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, transparent 40%, var(--bg-surface) 100%);
}

.status-badge {
  position: absolute;
  top: 0.75rem;
  right: 0.75rem;
  background: rgba(16, 185, 129, 0.9);
  color: #fff;
  font-size: 0.625rem;
  font-weight: 700;
  padding: 0.2rem 0.5rem;
  border-radius: var(--radius-sm);
  animation: pulse 1.5s infinite;
}

.card-info {
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.game-name {
  font-size: 1.125rem;
  font-weight: 700;
  color: #fff;
  margin-bottom: 0.2rem;
}

.game-genre {
  font-size: 0.75rem;
  color: var(--accent-cyan);
  margin-bottom: 1rem;
}

.game-meta {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  font-size: 0.75rem;
  color: var(--text-dim);
  margin-bottom: 1.25rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid var(--line-dim);
}

.action-row {
  margin-top: auto;
}

.play-btn {
  width: 100%;
}

.stop-btn {
  width: 100%;
  border-color: rgba(244, 63, 94, 0.4);
  color: var(--accent-rose);
}

.stop-btn:hover {
  background: rgba(244, 63, 94, 0.1);
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}
</style>
