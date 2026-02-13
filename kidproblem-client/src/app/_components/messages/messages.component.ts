import { Component, OnInit } from '@angular/core';
import { MessageService } from '@app/_services';
import { MatButtonModule } from '@angular/material/button';
import { NgIf, NgFor } from '@angular/common';


@Component({
    selector: 'app-messages',
    templateUrl: './messages.component.html',
    styleUrls: ['./messages.component.css'],
    standalone: true,
    imports: [NgIf, NgFor, MatButtonModule]
})
export class MessagesComponent implements OnInit {

  constructor(
    public messageService: MessageService) { }

  ngOnInit() {
  }

}
