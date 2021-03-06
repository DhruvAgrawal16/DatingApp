import { User } from './../_models/user';
import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = environment.apiUrl + 'auth/';
jwtHelper = new JwtHelperService();
decodedToken: any;
currentUser: User;

constructor(private http: HttpClient) { }

login(model: any){
  return this.http.post(this.baseUrl + 'login', model)
  .pipe(
    map((response: any) => {
        const user = response;
        if (user){
          localStorage.setItem('token', user.token);
          localStorage.setItem('userForListDto', JSON.stringify(user.userForListDto));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.userForListDto;
        }
      })
  );
}

register(model: any){
  return this.http.post(this.baseUrl + 'register', model);
}

loggedIn(){
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

}
