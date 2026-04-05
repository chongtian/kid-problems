import { Injectable, signal, computed } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingBusService {
  private counter = signal(0);
  readonly isLoading = computed(() => this.counter() > 0);

  start() {
    this.counter.update(n => n + 1);
  }

  stop() {
    this.counter.update(n => Math.max(0, n - 1));
  }
}