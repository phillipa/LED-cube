//All sizes in mm
$fn = 50;
pipe_od = 26.8; // 3/4in pipe
fitting_depth = 43; //socket for pipe
wall_thick = 7; //walls around socket

//Hole all the way through
hole_d = pipe_od - 5;

//Diameter of fitting
overall_d = pipe_od + (wall_thick * 2);
overall_r = overall_d/2;

//Length of fitting
internal_space = overall_d;
overall_l = internal_space + (fitting_depth * 2);

difference(){
rotate([45, 35,0]){
translate([0,0,-overall_l/2]){
    difference(){
        union(){
            //main outer cylinder (z_axis)
            cylinder(h=overall_l, r=overall_r);

            translate([0,0,overall_l/2]){
                //main outer cylinder (y axis)
                translate([0, overall_l/2, 0]){
                    rotate([90, 0, 0]){
                        cylinder(h=overall_l, r=overall_r);
                    }
                }

                //main outer cylinder (x axis)
                translate([-overall_l/2, 0, 0]){
                    rotate([0, 90, 0]){
                        cylinder(h=overall_l, r=overall_r);
                    }
                }
            }
        }
        union(){
            //main outer cylinder (z_axis)
            make_hole();

            translate([0,0,overall_l/2]){
                //main outer cylinder (y axis)
                translate([0, overall_l/2, 0]){
                    rotate([90, 0, 0]){
                        make_hole();
                    }
                }

                //main outer cylinder (x axis)
                translate([-overall_l/2, 0, 0]){
                    rotate([0, 90, 0]){
                        make_hole();
                    }
                }
            }
        }
    }
}
}

       translate([0,0,-53]){
    cube([overall_l, overall_l, 2], center=true);
}
}
module make_hole(){
    //holes inside it
    union(){
        cylinder(h=overall_l, r=hole_d/2);
        cylinder(h=fitting_depth, r=pipe_od/2); 
        translate([0,0,overall_l-fitting_depth]){
            cylinder(h=fitting_depth, r=pipe_od/2); 
        }
    }
}