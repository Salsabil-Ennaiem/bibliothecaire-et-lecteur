import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from '../auth.service';

@Injectable({
  providedIn: 'root'
})
export class DashboardSignalRService {
   private hubConnection?: signalR.HubConnection;
  private dashboardUpdateSubject = new BehaviorSubject<any>(null);

  dashboardUpdates$ = this.dashboardUpdateSubject.asObservable();

  constructor(private authService: AuthService) {}

  connect(biblioId: string): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5232/api/dashboard', {
        accessTokenFactory: () => this.authService.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('SignalR connected');
        this.hubConnection!.invoke('JoinBiblioGroup', biblioId);
        this.registerHandlers();
      })
      .catch(err => console.error('Error SignalR connection:', err));
  }

  private registerHandlers(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('UpdateRequested', (data) => {
      this.dashboardUpdateSubject.next(data);
    });

    this.hubConnection.on('Connected', (connectionId) => {
      console.log('Connected with id:', connectionId);
    });
  }

  requestDashboardUpdate(biblioId: string): void {
    this.hubConnection?.invoke('RequestDashboardUpdate', biblioId);
  }

  disconnect(biblioId: string): void {
    this.hubConnection?.invoke('LeaveBiblioGroup', biblioId);
    this.hubConnection?.stop();
  }
}