import { Injectable, OnInit } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ChartModel } from '../interfaces/chartModel.interface';
import { Message } from '../interfaces/messageModel.interface';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public data: ChartModel[];
  public message: Message;
  public messages: Message[] = [];

  private hubConnection: signalR.HubConnection;

  // Start the connection
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:5001/signalRHub')
      .configureLogging(signalR.LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.addTransferChartDataListener();
        this.addMessageListener();
      })
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  // Join the chat room
  public joinChatRoom = () => {
    this.hubConnection.invoke("JoinChatRoom").catch((ex) => {
      console.log(ex);
    });
  }

  // Leave the chat room
  public leaveChatRoom = () => {
    this.hubConnection.invoke("LeaveChatRoom").catch((ex) => {
      console.log(ex);
    });
  }

  // Add message to the chat
  public newMessage(message: string) {
    this.hubConnection.invoke("NewMessage", message).catch((ex) => {
      console.log(ex);
    });
  }

  // Add the chart data listener
  private addTransferChartDataListener = () => {
    this.hubConnection.on('BroadcastChartData', (data) => {
      this.data = data;
      console.log(data);
    });
  }

  // Add chat message listener
  private addMessageListener = () => {
    this.hubConnection.on('BroadcastMessage', (message: Message) => {
      this.message = message;
      this.messages.push(this.message);
      console.log(message);
    });
  }
}
