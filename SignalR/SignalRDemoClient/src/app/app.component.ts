import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CustomLogger } from './Models/CustomLogger';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'SignalRDemoClient';
  connection: signalR.HubConnection;
  stringBuilderconnection!: signalR.HubConnection;
  counter = 0;
  applicationName: string | null = "";

  firstName: string = '';
  lastName: string = '';
  submitted: boolean = false;

  //initailze the connection
  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      // .configureLogging(new CustomLogger())
      .withAutomaticReconnect()
      .withServerTimeout(30000)
      .withUrl("https://localhost:7190/hubs/view"
        , {
          transport: signalR.HttpTransportType.WebSockets
            | signalR.HttpTransportType.ServerSentEvents
        })
      .build();

    this.stringBuilderconnection
      = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7190/hubs/stringBuilderHub",
          { transport: signalR.HttpTransportType.WebSockets }
        )
        .configureLogging(signalR.LogLevel.Information)
        .build();
  }



  //handle the connection
  ngOnInit(): void {
    this.connection.start().then(
      () => this.success(),
      () => this.failed()
    );
    this.connection.on("updateViewCounter", (value: number) => {
      this.counter = value;
    });

    this.stringBuilderconnection
      .start()
      .then(
        () => this.submitForm(),
        () => console.log("string builder connetin failed")
      );

    this.stringBuilderconnection
      .on("getFullName", (val) => {
        alert(val);
      })
  }
  notify() {
    this.connection.send("NotifyWatching"); // اسم الفانكشن اللي في الباك اند
  }
  success() {
    console.log("connected");
    this.notify();
    // this.getApplicationName();
  }
  failed() {
    console.log("failed to connect with SignalR");
  }

  getApplicationName() {
    this.notify();
    console.log("Get Application Name");
    this.connection
      .send("getApplicationName")
      .then((val) => {
        console.log(val);
      })
  }

  submitForm() {
    this.submitted = true;
    this.stringBuilderconnection
      .send("getFullName", this.firstName, this.lastName)
  }
}
