import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AuthMenuGuard } from '@app/_guards';
import { MatDividerModule } from '@angular/material/divider';
import { RouterLink } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { NgIf } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { DisplayMessages } from '@app/_constants';

@Component({
  selector: 'kp-menu',
  templateUrl: './kp-menu.component.html',
  styleUrls: ['./kp-menu.component.css'],
  standalone: true,
  imports: [NgIf, MatButtonModule, MatMenuModule, RouterLink, MatDividerModule, MatToolbarModule, MatButtonModule, MatIconModule]
})
export class KpMenuComponent implements OnInit {

  @Input() username = '';
  @Input() version = '';
  @Output('sign-out') signOutEvent = new EventEmitter<boolean>();

  constants = DisplayMessages;

  constructor(
    public menuGuard: AuthMenuGuard) { }

  ngOnInit() {
    this.menuGuard.setEnableFlags();
  }

  signOut(){
    this.signOutEvent.emit(true);
  }

}
