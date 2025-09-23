import { CommonModule } from '@angular/common';
import { Component, ViewChild, EventEmitter, Output, OnInit, OnDestroy } from '@angular/core';
import { Popover } from 'primeng/popover';
import { PopoverModule } from 'primeng/popover';
import { EmpruntService } from '../../../Services/emprunt.service';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule, PopoverModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit, OnDestroy {
  @ViewChild('op') popover!: Popover;
  @Output() unreadCountChange = new EventEmitter<number>();

  notifications: { message: string; isRead: boolean }[] = [];
  unreadCount = 0;
  private subscription?: Subscription;

  constructor(private empruntService: EmpruntService) { }
  //10s=1000ms*10
  ngOnInit() {
    this.subscription = timer(0, 60000).subscribe(() => {
      this.empruntService.notifcation().subscribe({
        next: (response: string) => {
          console.log('Backend response:', response);
          // If you want to trigger UI updates or notifications, do it here
        },
        error: (err) => {
          console.error('Failed to load notifications', err);
        }
      });

    });
  }

  ngOnDestroy() {
    this.subscription?.unsubscribe();
  }

  open(event: Event) {
    this.popover.toggle(event);
    // Mark all as read when popover is opened
    this.notifications.forEach(n => n.isRead = true);
    this.updateUnreadCount();
  }

  private updateUnreadCount() {
    this.unreadCount = this.notifications.filter(n => !n.isRead).length;
    this.unreadCountChange.emit(this.unreadCount);
  }
}
