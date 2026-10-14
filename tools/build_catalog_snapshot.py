"""Regenerate the offline catalogue snapshot shipped with the Blazor client.

The storefront falls back to this file when the API is asleep, so it must stay in
sync with the seed table the API serves. Run it after editing CatalogSeedData.cs:

    python tools/build_catalog_snapshot.py

The output is deterministic: identifiers derive from the cover filename and
release dates are offsets from a fixed anchor, so regenerating an unchanged seed
table produces an identical file and a cart saved in localStorage survives it.
"""

from __future__ import annotations

import hashlib
import json
import re
import uuid
from datetime import datetime, timedelta, timezone
from decimal import Decimal
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent.parent
SEED_FILE = REPO_ROOT / "GameHaven.Infrastructure" / "Persistence" / "CatalogSeedData.cs"
SNAPSHOT_FILE = REPO_ROOT / "GameHaven.WebUI" / "wwwroot" / "data" / "catalog.json"

# Release dates are anchored rather than taken from the clock so the snapshot
# does not churn on every run. Only the relative order matters to the storefront.
RELEASE_ANCHOR = datetime(2026, 1, 1, tzinfo=timezone.utc)

# new("Title", "Genre", "Tagline", 8.99m, 0, 88, "Studio", "cover.png", 236),
ENTRY_PATTERN = re.compile(
    r'new\(\s*"([^"]*)"\s*,\s*"([^"]*)"\s*,\s*"([^"]*)"\s*,\s*'
    r"([0-9.]+)m\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*"
    r'"([^"]*)"\s*,\s*"([^"]*)"\s*,\s*(\d+)\s*\)'
)


def deterministic_id(cover: str) -> str:
    """Stable UUID for a catalogue row, keyed on its cover filename."""
    digest = hashlib.sha256(f"gamehaven:{cover}".encode("utf-8")).digest()
    return str(uuid.UUID(bytes=digest[:16], version=5))


def current_price(base_price: Decimal, discount: int) -> Decimal:
    """Mirror Game.CurrentPrice so offline and live prices agree exactly."""
    if discount <= 0:
        return base_price
    return base_price * (Decimal(1) - (Decimal(discount) / Decimal(100)))


def build() -> list[dict]:
    source = SEED_FILE.read_text(encoding="utf-8")
    matches = ENTRY_PATTERN.findall(source)

    if not matches:
        raise SystemExit(f"No catalogue entries matched in {SEED_FILE}")

    games = []

    for title, genre, tagline, price, discount, rating, studio, cover, days in matches:
        base_price = Decimal(price)
        discount_pct = int(discount)
        release = RELEASE_ANCHOR - timedelta(days=int(days))

        games.append(
            {
                "id": deterministic_id(cover),
                "title": title,
                "description": tagline,
                "basePrice": float(base_price),
                "currentPrice": float(current_price(base_price, discount_pct)),
                "discountPercentage": discount_pct,
                "coverImageUrl": cover,
                "developer": studio,
                "publisher": studio,
                "genre": genre,
                "ratingPercentage": int(rating),
                "releaseDate": release.strftime("%Y-%m-%dT%H:%M:%SZ"),
            }
        )

    return games


def main() -> None:
    games = build()

    SNAPSHOT_FILE.parent.mkdir(parents=True, exist_ok=True)
    with SNAPSHOT_FILE.open("w", encoding="utf-8", newline="\n") as handle:
        handle.write(json.dumps(games, ensure_ascii=False, indent=2) + "\n")

    print(f"Wrote {len(games)} entries to {SNAPSHOT_FILE.relative_to(REPO_ROOT)}")


if __name__ == "__main__":
    main()
