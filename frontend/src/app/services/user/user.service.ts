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
    .post('https://localhost:7084/api/auth/login', {
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
      localStorage.setItem('username', username)
      this.loggedIn = true;
      this.router.navigate(['']);
    })
    return  post;
      
  }

   logout() {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('username');
    localStorage.removeItem('token');
    this.userSubject.next(null);
    this.loggedIn = false;
    this.router.navigate(['/account/login']);
   }

  register(user: User) {
    console.log("registering",user.username,user.password,user);
    return this.http.post("api/user/adduser", user);
  }

  getUserPosts(username : string){
    //http request here

    return ["post 1", "post 2", "post 3", "post 4","post 5","post 6","post 7","post 8","post 9","post 10",];
  }

  getUserComments(username : string){
    //http request here

    return ["comment 1", "comment 2", "comment 3"];
  }
  
  getUserFavouritePosts(username : string){
    //http request here

    return ["favourite post 1", "favourite post 2", "favourite post 3"];
  }
 
}
