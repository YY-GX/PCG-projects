using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sd_surface;
using System;
namespace space_craft {
    class SpaceCraft {
        private Vector3 central_pos;
        private System.Random rnd;
        private SC_Head sc_head;
        private SC_Body sc_body;
        private SC_Window sc_window;
        private SC_Tail sc_tail;
        private SC_Wing sc_wing;
        public SpaceCraft(Vector3 central_pos, System.Random rnd) {
            this.central_pos = central_pos;
            this.rnd = rnd;
            this.assemble();
        }
        // Assemble different parts of the spacecraft
        public void assemble() {
            // Position for each part
            Vector3 pos_head = new Vector3(central_pos.x, central_pos.y, central_pos.z - 13f);
            Vector3 pos_body = new Vector3(central_pos.x, central_pos.y, central_pos.z - 30f);
            Vector3 pos_window = new Vector3(central_pos.x, central_pos.y - .5f, central_pos.z - 22f);
            Vector3 pos_tail = new Vector3(central_pos.x, central_pos.y, central_pos.z - 30f);
            Vector3 pos_wing = new Vector3(central_pos.x, central_pos.y, central_pos.z - 7f);

            // Create parts
            // Create head
            this.sc_head = new SC_Head(pos_head, this.rnd);
            // Create body
            this.sc_body = new SC_Body(pos_body, this.rnd);
            // Create window
            this.sc_window = new SC_Window(pos_window, this.rnd);
            // Create tail
            this.sc_tail = new SC_Tail(pos_tail, this.rnd);
            // Create wing
            this.sc_wing = new SC_Wing(pos_wing, this.rnd);
        }
        public void update()
        {
            this.sc_wing.update(0.05f);
            this.sc_tail.update(0.5f);
        }
    }
}