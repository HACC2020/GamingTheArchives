import { Injectable, TemplateRef } from '@angular/core';

type NotificationOptions = { delay?: number, className?: string };

@Injectable({ providedIn: 'root' })
export class NotificationService {
  notifications: Array<{ textOrTpl: string | TemplateRef<any> } & NotificationOptions> = [];

  show(textOrTpl: string | TemplateRef<any>, options: NotificationOptions = {}): any {
    let notification = { textOrTpl, ...options };
    this.notifications.push(notification);
    return notification;
  }

  info(textOrTpl: string | TemplateRef<any>, options: NotificationOptions = {}): any {
    if (!options.className) {
      options.className = 'notification-info';
    } else {
      options.className += ' notification-info';
    }

    return this.show(textOrTpl, options);
  }

  success(textOrTpl: string | TemplateRef<any>, options: NotificationOptions = {}): any {
    if (!options.className) {
      options.className = 'notification-success';
    } else {
      options.className += ' notification-success';
    }

    return this.show(textOrTpl, options);
  }

  warning(textOrTpl: string | TemplateRef<any>, options: NotificationOptions = {}): any {
    if (!options.className) {
      options.className = 'notification-warning';
    } else {
      options.className += ' notification-warning';
    }

    return this.show(textOrTpl, options);
  }

  error(textOrTpl: string | TemplateRef<any>, options: NotificationOptions = {}): any {
    if (!options.className) {
      options.className = 'notification-error';
    } else {
      options.className += ' notification-error';
    }

    return this.show(textOrTpl, options);
  }

  remove(notification) {
    this.notifications = this.notifications.filter(t => t !== notification);
  }
}
