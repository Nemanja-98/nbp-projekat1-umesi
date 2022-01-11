import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from 'src/app/models/user';


interface logInSessionToken{
  token : string;
}
@Injectable({
  providedIn: 'root'
})
export class UserService {

  private userSubject: BehaviorSubject<User>;
  public user: Observable<User>;
  public loggedIn : boolean = false;
  constructor(
    private router: Router,
    private http: HttpClient,
  ) {
    this.userSubject = new BehaviorSubject<User>(
      JSON.parse(localStorage.getItem('currentUser'))
    );
    this.user = this.userSubject.asObservable();
  }

  public get userValue(): User {
    return this.userSubject.value;
  }

   login(username: string, password: string) {
    console.log("logging in");
    
    const post =  this.http
    .post('/user/login', {
       "Username":username,
      "Password": password,
    },{headers: {
      'Access-Control-Allow-Origin': '*',
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    }});;
    
    post.toPromise().then((data : logInSessionToken) => {
      console.log("ejta",data);
      const token = data.token;
      console.log("tokenche",token);
      localStorage.setItem('token', token );
      this.loggedIn = true;
    })
    return  post;
      
  }

   logout() {
    localStorage.removeItem('currentUser');
    this.userSubject.next(null);
    this.loggedIn = false;
    this.router.navigate(['/account/login']);
   }

  register(user: User) {
    console.log("registering",user.username,user.password,user);
    return this.http.post("user/register", user);
  }
 
}
