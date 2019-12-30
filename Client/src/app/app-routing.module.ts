import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ChartComponent } from './liveChart/liveChart.component';
import { ChatComponent } from './chat/chat.component';
import { HomeComponent } from './home/home.component';


const routes: Routes = [
  { path: 'Home', component: HomeComponent },
  { path: 'LiveChart', component: ChartComponent },
  { path: 'Chat', component: ChatComponent },
  { path: '', redirectTo: '/Home', pathMatch: 'full' },
  { path: '**', component: ChartComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
