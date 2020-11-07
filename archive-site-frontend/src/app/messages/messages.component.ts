//this component prints the newest messages on the activity feed.

import { Component, OnInit } from '@angular/core';
import { MessageService } from '../services/message.service';
import { USERS } from '../mock-data/mock-users';

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
//Create a message for each user in the mock user file, just to fill
//the Activity Feed with some content.
  getMessage(): void{
    for (let user of USERS) {
      this.messageService.add(user.DisplayName + " was added.");
    }
  }

}