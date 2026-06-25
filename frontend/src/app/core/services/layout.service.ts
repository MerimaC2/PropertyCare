import { inject, Injectable, Signal } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

/** Responsive breakpoint state, read as a signal in layout templates (zoneless-friendly). */
@Injectable({ providedIn: 'root' })
export class LayoutService {
  private readonly breakpointObserver = inject(BreakpointObserver);

  /** True on phones / narrow tablets, where the inline toolbar nav collapses into a menu. */
  readonly isHandset: Signal<boolean> = toSignal(
    this.breakpointObserver.observe('(max-width: 820px)').pipe(map(result => result.matches)),
    { initialValue: false }
  );
}
