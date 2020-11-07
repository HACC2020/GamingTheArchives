import { Component, OnInit, TemplateRef } from '@angular/core';
import { NotificationService } from 'src/app/services/notification-service';

@Component({
  selector: 'app-notifications',
  template: `
    <ngb-toast
      *ngFor="let notice of notificationService.notifications"
      [class]="notice.className"
      [autohide]="true"
      [delay]="notice.delay || 5000"
      (hide)="notificationService.remove(notice)">
      <ng-template [ngIf]="isTemplate(notice)" [ngIfElse]="text">
        <ng-template [ngTemplateOutlet]="notice.textOrTpl"></ng-template>
      </ng-template>

      <ng-template #text>{{ notice.textOrTpl }}</ng-template>
    </ngb-toast>
  `,
  host: { '[class.ngb-toasts]': 'true' }
})
export class NotificationsContainerComponent implements OnInit {
  constructor(public notificationService: NotificationService) {
  }

  isTemplate(toast): boolean {
    return (toast.textOrTpl instanceof TemplateRef);
  }

  ngOnInit(): void {
  }
}
