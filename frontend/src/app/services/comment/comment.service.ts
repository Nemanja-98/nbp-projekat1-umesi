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

  postComment(comment: Comment, postId: string) : Observable<any> 
  {
    const url = `${this.apiUrl}AddComment/${postId}`
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
        'Accept': '*',
        'Authorization': localStorage.getItem("token")
    })
  }

  deleteComment(postId: number, index: number) : Observable<any>
  {
    const url = `${this.apiUrl}DeleteComment/${postId}/${index}`
    return this.http.delete(url, {headers: this.returnBaseHttpHeaders(), responseType: "text"});
  }

  updateComment(postId: number, index: number, newContent: string): Observable<Comment> 
  {
    const url = `${this.apiUrl}UpdateComment`
    return this.http.put<Comment>(url, {
      "recipeId": postId,
      "index": index,
      "comment": {
        "userRef": localStorage.getItem("username"),
        "description": newContent,
        "isDeleted": 0
      }
    }, {headers: this.returnBaseHttpHeaders()} )
  }

  getCommentsForRecipe(postId: number): Observable<Comment[]>{
    const url = `${this.apiUrl}GetCommentsForRecipe/${postId}`;
    return this.http.get<Comment[]>(url, {headers: this.returnBaseHttpHeaders()});
  }
}