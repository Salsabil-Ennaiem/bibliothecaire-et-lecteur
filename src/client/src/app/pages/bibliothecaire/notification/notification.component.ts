import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { Popover } from 'primeng/popover';
import { PopoverModule } from 'primeng/popover';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [ CommonModule,Popover,PopoverModule ],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css'] // fixed from styleUrl to styleUrls
})
export class NotificationComponent {
    @ViewChild('op') popover!: Popover; 
     open(event: Event) {
    this.popover.toggle(event);
  }

  notifications = Array.from({ length: 25 }).map((_, i) => ({
    message: `Notification ${i + 1}`
  }));

}
