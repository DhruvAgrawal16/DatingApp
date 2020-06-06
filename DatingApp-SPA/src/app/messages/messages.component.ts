import { PaginatedResult } from 'src/app/_models/pagination';
import { ActivatedRoute } from '@angular/router';
import { Pagination } from './../_models/pagination';
import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';


@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
messages: Message[];
pagination: Pagination;
messageContainer = 'Unread';

  constructor(private userService: UserService, private authService: AuthService,
              private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      // tslint:disable-next-line: no-string-literal
      this.messages = data['messages'].result;
      // tslint:disable-next-line: no-string-literal
      this.pagination = data['messages'].pagination;
    });
    this.loadMessages();
  }

  loadMessages(){
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.pagination.currentPage, this.pagination.itemsPerPage, this.messageContainer)
      .subscribe((res: PaginatedResult<Message[]>) => {
        this.messages = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error);
      } );

  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
}
}
