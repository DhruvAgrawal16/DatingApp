import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
@Input() user: User;
  constructor(private userService: UserService,
              private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(recipientId: number){
    this.userService.sendLike(this.authService.decodedToken.nameid, recipientId)
    .subscribe(data => {
      this.alertify.success('you have liked ' + this.user.knownAs);
    }, error => {
      this.alertify.error(error);
    });
  }
}
