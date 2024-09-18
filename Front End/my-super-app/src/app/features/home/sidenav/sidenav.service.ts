import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SidenavService {

  constructor() { }
  private sidenavToggle = new Subject<void>();

  sidenavToggle$ = this.sidenavToggle.asObservable();

  toggle() {
    this.sidenavToggle.next();
  }
}
