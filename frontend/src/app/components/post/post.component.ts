import { Component, OnInit, Input } from '@angular/core';
import { Comment } from 'src/app/models/comment';
import { Post } from 'src/app/models/post';
import { PostService } from 'src/app/services/post/post.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-post',
  templateUrl: './post.component.html',
  styleUrls: ['./post.component.css']
})
export class PostComponent implements OnInit {

  post: Post = new Post(1, "Miladin", "Gulas", "Lako se sprema", ["ulje", "jaja", "krompir", "meso"] ,[new Comment("Kika", "Dobro", 0), new Comment("Micko", "Super supica", 0)])
  id: string = "";


  constructor(private postService: PostService, private _Activatedroute:ActivatedRoute) { }

  ngOnInit(): void {

    this.id=this._Activatedroute.snapshot.paramMap.get("id");
    this.postService.getRecipeById(this.id).subscribe((resultPost) => {
      this.post = resultPost
      this.post.comments = this.post.comments.filter(x => x.isDeleted === 0);
      console.log(this.post);
    })
  }

  loggedIn(): boolean{
    return localStorage.getItem("username") ? true : false
  }
}
