$fn = 80;
pipe_od = 26.8; // 3/4in pipe
fitting_depth = 25.5; //Depth of hole
foam_thick = 62; //white noodle foam
mate_d = 15;

difference(){
   union(){
      //main body of tube
      translate([-foam_thick/2, 0, 50]){
         rotate([0,90,0]){
            cylinder(h=foam_thick, r = 20);
         }
      }

      //Down tube for PVC
      cylinder(h=fitting_depth*2, r = 20);
      
      translate([30, 0, 50]){
         rotate([0,90,0]){
            cylinder(h = 20, r = mate_d);
         }
      }
    
    

   }

   //Hole for the PVC pipe
   cylinder(h = fitting_depth*2, r=(pipe_od-3)/2);
   cylinder(h = fitting_depth, r=pipe_od/2);

   //Hole to mate with adjacent
//   translate([-foam_thick/2, 0, 50]){
//      rotate([0,90,0]){
//         cylinder(h = 50, r = mate_d+0.3);
//      }
//   } 
//   
   //Hole to mate with adjacent, now with chamfer to build without support
   mink_rad = 5;
   fit_fudge = 0.3;
   minkowski(){      
      translate([-foam_thick/2, 0, 50]){
         rotate([0,90,0]){
            cylinder(h = 50, r = mate_d+fit_fudge-mink_rad);
         }
      } 
      sphere(mink_rad);
   }
   
   //Hole all the way through
   translate([-40, 0, 50]){
      rotate([0,90,0]){
         cylinder(h = 100, r = mate_d-3);
      }
   } 

}
