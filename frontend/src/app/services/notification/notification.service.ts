import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor() {}

  public connection;
  public recipe = [];
  public users = [];

  beginConnection(token) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/notification', { accessTokenFactory: () => token,
        transport: signalR.HttpTransportType.LongPolling })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    const start = async () => {
      try {
        await this.connection.start();
        console.log('SignalR Connected for notificationHub.');
      } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
      }
    }

    this.connection.onclose(start);

    // Start the connection.
    start();

    this.connection.on('GetRecipe', (recipe) => {
      console.log('notificationHub recipe is', recipe);

      recipe.value.recipeList.forEach((element) => {
        this.recipe.push(element);
      });
      console.log('notificationHub array', this.recipe);
    });

    this.connection.on('UserConnected', (user) => {
      this.users.push(user);
      console.log('notificationHub user pushed', user);
    });
    
    this.connection.on('UserDisconnected', (user) => {
      this.users = this.users.filter((el) => el != user);
      console.log('chathub user removed', user, 'from this array', this.users);
    });

    return of(this.users);
  }
}