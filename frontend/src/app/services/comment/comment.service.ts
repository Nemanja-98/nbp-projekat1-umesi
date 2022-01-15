import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaderResponse, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http'
import { Comment } from 'src/app/models/comment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  private apiUrl = 'https://localhost:7084/api/Comment/'

  constructor(private http: HttpClient) { }

  postComment(comment: Comment, recipeId: string) : Observable<any> {
    const url = `${this.apiUrl}AddComment/${recipeId}`
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

  deleteComment(recipeId: number, index: number) {

    const options = {
      headers: this.returnBaseHttpHeaders(),
      body: {
        "recipeId": recipeId,
        "index": index
      },
    };

    const url = `${this.apiUrl}DeleteComment`
    return this.http.delete(url, options);
  }
}