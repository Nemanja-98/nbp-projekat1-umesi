import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    async function nesto(){
      console.log("nesto CALLED");
      const req1 = setTimeout(() => {
        console.log("fake api call finished");
        return 1;
      }, 1000*60*4);
      const req2 = setTimeout(() => {
        console.log("request 2 finished");
        fetch('https://jsonplaceholder.typicode.com/todos/1')
        .then(response => response.json())
        .then(json => console.log(json))
        return 2;
      }, 1000*58*4);
      // request 1 se zavrsava nakon 3 minuta
      //request 2 se zavrsava 2 sekunde ranije
      // proveriti network stalled tab.
      const response = await Promise.race([req1,req2])
      console.log("response is",response);
    }
    nesto();
  }

}
