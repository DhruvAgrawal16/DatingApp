import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { AlertifyService } from './../../_services/alertify.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRouteSnapshot, ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
@ViewChild('editForm', {static: true}) editForm: NgForm;
  user: User;
  constructor(private route: ActivatedRoute, private userService: UserService, private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      // tslint:disable-next-line: no-string-literal
      this.user = data['user'];
    });
  }
  updateUser(){
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user)
    .subscribe(next => {
      this.alertify.success('Profile Updated Successfully');
      this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }
  updateUserMainPhoto(photoUrl){
    this.user.photoUrl = photoUrl;
  }

}
