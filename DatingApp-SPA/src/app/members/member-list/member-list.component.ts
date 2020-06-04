import { Pagination } from './../../_models/pagination';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
users: User[];
page = 1;
itemsPerPage = 5;
pagination: Pagination;


  constructor(private userService: UserService,
              private alerify: AlertifyService) { }

  ngOnInit() {
    this.loadUsers();
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers2();
}
  loadUsers(){
    this.userService.getUsers(this.page, this.itemsPerPage).subscribe((users: PaginatedResult<User[]>) => {
    this.users = users.result;
    this.pagination = users.pagination; }
    , error => {
      this.alerify.error(error);
  });
}
loadUsers2(){
  this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage).subscribe((users: PaginatedResult<User[]>) => {
  this.users = users.result;
  this.pagination = users.pagination; }
  , error => {
    this.alerify.error(error);
});
}

}
