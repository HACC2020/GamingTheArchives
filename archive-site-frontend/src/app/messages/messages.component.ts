//this component prints the newest messages on the activity feed.

import { Component, OnInit } from '@angular/core';
import { MessageService } from '../services/message.service';
import { USERS } from '../mock-data/mock-users';
import { stringToKeyValue } from '@angular/flex-layout/extended/typings/style/style-transforms';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})

export class MessagesComponent implements OnInit {

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
    this.getMessage();

  }

  getMessage(): void{
    for (let user of USERS) {
      this.messageService.add(user.DisplayName + " was added.");
    }
  }

}