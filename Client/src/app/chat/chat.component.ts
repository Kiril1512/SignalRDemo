import { Component, OnInit, OnDestroy } from '@angular/core';
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy {

  constructor(public signalRService: SignalRService) { }

  ngOnInit() {
    // Join chat room
    this.signalRService.joinChatRoom();
  }

  newMessage(message: string) {
    // Add nem message to the chat
    this.signalRService.newMessage(message);
  }

  ngOnDestroy(): void {
    // Leave chat room
    this.signalRService.leaveChatRoom();
  }
}
