import { DOCUMENT } from '@angular/common';
import { Injectable, inject, signal } from '@angular/core';

/** Toggles between the light and dark color themes and remembers the choice. */
@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly storageKey = 'propertycare-theme';

  /** Read in templates so the toggle icon updates reactively (zoneless). */
  readonly isDark = signal(false);

  constructor() {
    const saved = localStorage.getItem(this.storageKey);
    this.applyDark(saved === 'dark');
  }

  toggle(): void {
    this.applyDark(!this.isDark());
  }

  private applyDark(dark: boolean): void {
    this.isDark.set(dark);
    this.document.body.classList.toggle('dark-theme', dark);
    localStorage.setItem(this.storageKey, dark ? 'dark' : 'light');
  }
}
