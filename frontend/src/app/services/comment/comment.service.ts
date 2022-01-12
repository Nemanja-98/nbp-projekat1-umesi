import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaderResponse, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http'
import { Comment } from 'src/app/models/comment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  private apiUrl = 'https://localhost:7084/api/Comment/AddComment/'

  constructor(private http: HttpClient) { }

  postComment(comment: Comment, recipeId: string) : Observable<any> {
    const url = `${this.apiUrl}${recipeId}`
    return this.http.post(url, {
      "userRef": comment.userRef,
      "description": comment.description,
      "isDeleted": 0
    } , {headers: this.returnBaseHttpHeaders(), responseType: "text"});
  }

  returnBaseHttpHeaders() : HttpHeaders
  {
    return new HttpHeaders({
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*',
        'Accept': '*'
    })
  }
}