import { Component, OnInit } from '@angular/core';
import { SignalRService } from './services/signal-r.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'SignalR-Demo';

  constructor(public signalRService: SignalRService) { }

  ngOnInit() {
    // Start SignalR connection
    this.signalRService.startConnection();
  }
}
