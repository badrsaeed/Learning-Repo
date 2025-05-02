import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'SignalRDemoClient';
  connection;
  counter = 0;

  //initailze the connection
  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7190/hubs/view")
      .build();
  }



  //handle the connection
  ngOnInit(): void {
    this.connection.start().then(this.success, this.failed);
    this.connection.on("viewCountUpdate", (value: number) => {
      this.counter = value;

    });
  }
  notify() {
    this.connection.send("notifyWatching");
  }
  success() {
    console.log("connected");
    this.notify();
  }
  failed() {
    console.log("failed to connect with SignalR");
  }
}
