/*this service creates a string of messages that will appear in "activity feed" aka messages component.
*this service is injected into the messages component, which can be dropped onto any page to show
*     the array of messages.
*this service can also be injected into any component that you want to send a message from
*     to the "Activity Feed".
*/

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  messages: string[] = [];

  add(message: string) {
    this.messages.push(message);
  }

  clear() {
    this.messages = [];
  }

}